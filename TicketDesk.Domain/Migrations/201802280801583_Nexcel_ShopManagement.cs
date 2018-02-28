namespace TicketDesk.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Nexcel_ShopManagement : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ShopManagements",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ShopName = c.String(nullable: false, maxLength: 100),
                        ShopCode = c.String(maxLength: 50),
                        ContactName = c.String(maxLength: 70),
                        ContactNumber = c.String(maxLength: 20),
                        Email = c.String(maxLength: 254),
                        Address = c.String(maxLength: 500),
                        LastUpdatedBy = c.String(),
                        LastUpdatedDate = c.DateTime(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.ShopManagements");
        }
    }
}
