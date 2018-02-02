namespace TicketDesk.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Nexcel_Nap_User_Setting : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.NapUserSettings",
                c => new
                    {
                        UserId = c.Int(nullable: false, identity: true),
                        Settings = c.String(),
                        ModifiedDate = c.DateTime(nullable: false),
                        ModifiedBy = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.NapUserSettings");
        }
    }
}
