namespace TicketDesk.Domain.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class Nexcel_AddMailToTicketCategory : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TicketCategories", "MailTo", c => c.String());
            AddColumn("dbo.TicketCategories", "MailCcTo", c => c.String());
            AddColumn("dbo.TicketCategories", "MailBccTo", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.TicketCategories", "MailBccTo");
            DropColumn("dbo.TicketCategories", "MailCcTo");
            DropColumn("dbo.TicketCategories", "MailTo");
        }
    }
}
