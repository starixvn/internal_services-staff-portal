namespace TicketDesk.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Nexcel_Announcement : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Announcements",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Message = c.String(maxLength: 2000),
                        CreatedDateTime = c.DateTime(nullable: false),
                        PublishedDateTime = c.DateTime(nullable: false),
                        ProjectId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        State = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Announcements");
        }
    }
}
