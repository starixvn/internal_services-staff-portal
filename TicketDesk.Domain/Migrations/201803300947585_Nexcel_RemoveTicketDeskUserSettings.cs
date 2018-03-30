namespace TicketDesk.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Nexcel_RemoveTicketDeskUserSettings : DbMigration
    {
        public override void Up()
        {
            DropTable("dbo.UserSettings");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.UserSettings",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 256),
                        ListSettingsJson = c.String(),
                        SelectedProjectId = c.Int(),
                    })
                .PrimaryKey(t => t.UserId);
            
        }
    }
}
