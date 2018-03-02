namespace TicketDesk.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class Nexcel_UpdateShops : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Shops", "Address", c => c.String(maxLength: 500));
            AddColumn("dbo.Shops", "LastUpdatedBy", c => c.String());
            AddColumn("dbo.Shops", "LastUpdatedDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Shops", "IsActive", c => c.Boolean(nullable: false));
            DropColumn("dbo.Shops", "Version");
        }

        public override void Down()
        {
            AddColumn("dbo.Shops", "Version", c => c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "timestamp"));
            DropColumn("dbo.Shops", "IsActive");
            DropColumn("dbo.Shops", "LastUpdatedDate");
            DropColumn("dbo.Shops", "LastUpdatedBy");
            DropColumn("dbo.Shops", "Address");
            DropColumn("dbo.Shops", "Email");
            DropColumn("dbo.Shops", "ContactNumber");
            DropColumn("dbo.Shops", "ContactName");
            DropColumn("dbo.Shops", "ShopCode");
        }
    }
}
