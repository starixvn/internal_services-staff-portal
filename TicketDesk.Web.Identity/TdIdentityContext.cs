namespace TicketDesk.Web.Identity
{
    using Microsoft.AspNet.Identity.EntityFramework;
    using Model;
    using System.Data.Entity;

    public class TdIdentityContext : IdentityDbContext<TicketDeskUser>
    {
        public TdIdentityContext()
            : base("TicketDesk", true)
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // we have to call the base modelbuilder first, or the new names get overridden by the base
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TicketDeskUser>().ToTable("IdentityUsers");
            modelBuilder.Entity<TicketDeskRole>().ToTable("IdentityRoles");
            modelBuilder.Entity<IdentityRole>().ToTable("IdentityRoles");
            modelBuilder.Entity<IdentityUserRole>().ToTable("IdentityUserRoles");
            modelBuilder.Entity<IdentityUserLogin>().ToTable("IdentityUserLogins");
            modelBuilder.Entity<IdentityUserClaim>().ToTable("IdentityUserClaims");
        }
    }
}