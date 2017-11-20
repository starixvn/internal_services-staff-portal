namespace TicketDesk.Web.Identity
{
    using Migrations;
    using System.Data.Entity;

    public class TdIdentityDbInitializer : MigrateDatabaseToLatestVersion<TdIdentityContext, Configuration>
    {
        // no implementation, defined here to simplify and unify naming conventions and usage patterns
    }
}