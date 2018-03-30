namespace TicketDesk.Domain.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class Nexcel_ChangeAnnouncementStatusToType : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Announcements", "Type", c => c.Int(nullable: false));
            DropColumn("dbo.Announcements", "Status");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Announcements", "Status", c => c.Int(nullable: false));
            DropColumn("dbo.Announcements", "Type");
        }
    }
}
