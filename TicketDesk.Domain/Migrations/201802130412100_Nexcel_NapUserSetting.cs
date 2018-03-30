namespace TicketDesk.Domain.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class Nexcel_NapUserSetting : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.NapUserSettings",
                c => new
                    {
                        UserId = c.Int(nullable: false),
                        JsonSetting = c.String(),
                    })
                .PrimaryKey(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.NapUserSettings");
        }
    }
}
