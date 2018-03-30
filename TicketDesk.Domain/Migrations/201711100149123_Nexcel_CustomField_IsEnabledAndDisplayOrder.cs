namespace TicketDesk.Domain.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class Nexcel_CustomField_IsEnabledAndDisplayOrder : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CustomFields", "IsEnabled", c => c.Boolean(nullable: false, defaultValue: true));
            AddColumn("dbo.CustomFields", "DisplayOrder", c => c.Int(nullable: false, defaultValue: 0));
        }
        
        public override void Down()
        {
            DropColumn("dbo.CustomFields", "DisplayOrder");
            DropColumn("dbo.CustomFields", "IsEnabled");
        }
    }
}
