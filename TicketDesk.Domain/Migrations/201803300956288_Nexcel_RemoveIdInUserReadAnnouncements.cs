namespace TicketDesk.Domain.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class Nexcel_RemoveIdInUserReadAnnouncements : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.UserReadAnnouncements");
            AddPrimaryKey("dbo.UserReadAnnouncements", new[] { "AnnouncementId", "UserId" });
            DropColumn("dbo.UserReadAnnouncements", "Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.UserReadAnnouncements", "Id", c => c.Int(nullable: false, identity: true));
            DropPrimaryKey("dbo.UserReadAnnouncements");
            AddPrimaryKey("dbo.UserReadAnnouncements", "Id");
        }
    }
}
