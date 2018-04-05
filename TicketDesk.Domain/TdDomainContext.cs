namespace TicketDesk.Domain
{
    using System.Data.Entity;
    using System.Data.Entity.Core.Objects;
    using System.Data.Entity.Infrastructure;
    using Model;

    public sealed class TdDomainContext : DbContext
    {
        ////private UserSettingsManager _userSettingsManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="TdDomainContext"/> class.
        /// </summary>
        /// <remarks>
        /// The securityProvider parameter can be left null; however, this should
        /// be reserved only for back-end and automated functionality that runs
        /// outside of a user's context (e.g. migrations)
        /// </remarks>
        /// <param name="securityProvider">The security provider.</param>
        public TdDomainContext(TdDomainSecurityProviderBase securityProvider)
            : this()
        {
            SecurityProvider = securityProvider;
        }

        public TdDomainContext()
            : base("name=TicketDesk")
        {
            // some dbsets are internal, manually initialize for use by EF
            ApplicationSettings = Set<ApplicationSetting>();

            // TODO: This is only public because it is used by migrations and related functions
            // This ctor can be removed or made internal by creating a class that implements IDbContextFactory<TicketDeskContext>
            // As I understand it, if the factory exists EF will use it instead of looking for a public ctor with no params
        }

        public TdDomainSecurityProviderBase SecurityProvider { get; }

        public DbSet<Shop> Shops { get; set; }

        public DbSet<TicketEvent> TicketEvents { get; set; }

        public DbSet<Ticket> Tickets { get; set; }

        public DbSet<TicketTag> TicketTags { get; set; }

        public DbSet<TicketSubscriber> TicketSubscribers { get; set; }

        public DbSet<TicketEventNotification> TicketEventNotifications { get; set; }

        public DbSet<TicketCategory> TicketCategories { get; set; }

        // These DbSets contain json serialized content. Callers cannot use standard LINQ to Entities
        // expressions with these safely. Marking internal to prevent callers having direct access
        // We'll provide a thin layer of abstraction for safely handling external interactions instead.
        internal DbSet<ApplicationSetting> ApplicationSettings { get; set; }

        public DbSet<CustomFieldType> CustomFieldTypes { get; set; }

        public DbSet<CustomField> CustomFields { get; set; }

        public DbSet<CustomFieldValue> CustomFieldValues { get; set; }

        public DbSet<TicketCategoryCustomField> TicketCategoryCustomField { get; set; }

        public DbSet<UploadedUserId> UploadedUserIds { get; set; }

        public DbSet<Announcement> Announcements { get; set; }

        public DbSet<Reference> References { get; set; }

        public DbSet<NapUserSetting> NapUserSettings { get; set; }

        public DbSet<CustomFieldListValue> CustomFieldListValues { get; set; }

        public DbSet<UserReadAnnouncement> UserReadAnnouncements { get; set; }

        /// <summary>
        /// Gets or sets the application settings specific to ticketdesk.
        /// </summary>
        /// <value>The application settings.</value>
        public ApplicationSetting TicketDeskSettings
        {
            // TODO: these change infrequently, cache these
            get
            {
                var aps = ApplicationSettings.GetTicketDeskSettings();

                // this should only ever happen once, but if no settings are in DB, make default set
                if (aps == null)
                {
                    aps = new ApplicationSetting();

                    // do this on another instance, because we don't want to commit other things that may be tracking on this instance
                    // this feels like cheating :)
                    using (var tempContext = new TdDomainContext())
                    {
                        ApplicationSettings.Add(aps);
                        tempContext.SaveChanges();
                    }
                }

                return aps;
            }

            set
            {
                var oldSettings = ApplicationSettings.GetTicketDeskSettings();
                if (oldSettings != null)
                {
                    ApplicationSettings.Remove(oldSettings);
                }

                ApplicationSettings.Add(value);
            }
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TicketEventNotification>()
                .HasRequired(c => c.TicketSubscriber)
                .WithMany()
                .WillCascadeOnDelete(false);

            modelBuilder.ComplexType<ApplicationSelectListSetting>()
                .Property(p => p.Serialized)
                .HasColumnName("SelectListSettingsJson");

            modelBuilder.ComplexType<ApplicationPermissionsSetting>()
                .Property(p => p.Serialized)
                .HasColumnName("PermissionsSettingsJson");

            modelBuilder.ComplexType<ApplicationSecuritySetting>()
               .Property(p => p.Serialized)
               .HasColumnName("SecuritySettingsJson");

            modelBuilder.ComplexType<ClientSetting>()
               .Property(p => p.Serialized)
               .HasColumnName("ClientSettingsJson");
        }

        /// <summary>
        /// Gets an object query for the specified type
        /// </summary>
        /// <remarks>
        /// This is used for applying filter and sort using ESQL. This may present some challenges
        /// when EF 7 is finalized, as it will no longer support ESQL nor be backed by ObjectContext
        /// </remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity">The entity for which an object query is needed.</param>
        /// <returns>ObjectQuery&lt;T&gt;.</returns>
        public ObjectQuery<T> GetObjectQueryFor<T>(IDbSet<T> entity) where T : class
        {
            var oc = ((IObjectContextAdapter)this).ObjectContext;
            return oc.CreateObjectSet<T>();
        }
    }
}