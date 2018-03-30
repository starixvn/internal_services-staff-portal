namespace TicketDesk.Domain.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class Nexcel_AdditionalEmail : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tickets", "AdditionalMail", c => c.String());
            AddColumn("dbo.TicketCategories", "IsAllowAdditionalMail", c => c.Boolean(nullable: false));
            DropColumn("dbo.CustomFields", "CustomFieldListValueId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.CustomFields", "CustomFieldListValueId", c => c.Int(nullable: false));
            DropColumn("dbo.TicketCategories", "IsAllowAdditionalMail");
            DropColumn("dbo.Tickets", "AdditionalMail");
        }
    }
}
