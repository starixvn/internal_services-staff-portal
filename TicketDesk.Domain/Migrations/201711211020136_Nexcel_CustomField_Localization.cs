namespace TicketDesk.Domain.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class Nexcel_CustomField_Localization : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CustomFields", "Name_vi", c => c.String());
            AddColumn("dbo.CustomFields", "Values_vi", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.CustomFields", "Values_vi");
            DropColumn("dbo.CustomFields", "Name_vi");
        }
    }
}
