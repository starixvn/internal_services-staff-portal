namespace TicketDesk.Web.Client
{
    using Domain;
    using Identity;
    using System.Data.Entity;

    public partial class Startup
    {
        // TODO: while it is convienient to have all this set here,
        // it may be more consistent to split the sub-domain db init up --perhaps move each into their own domain (identity and push notifications)
        public static void ConfigureDatabase()
        {
            // kill initializer - features that may need one will reset this later
            Database.SetInitializer<TdDomainContext>(null);
            Database.SetInitializer<TdIdentityContext>(null);

            // set the std migrator for identity
            if (DatabaseConfig.IsDatabaseReady)
            {
                Database.SetInitializer(new TdIdentityDbInitializer());
            }

            DatabaseConfig.RegisterDatabase();
        }
    }
}