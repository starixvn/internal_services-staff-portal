namespace TicketDesk.Domain.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class Nexcel_RenameProjectToShop : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Tickets", "ProjectId", "dbo.Projects");
            DropIndex("dbo.Tickets", new[] { "ProjectId" });
            CreateTable(
                "dbo.Shops",
                c => new
                {
                    ShopId = c.Int(nullable: false, identity: true),
                    ShopName = c.String(nullable: false, maxLength: 100),
                    ShopDescription = c.String(maxLength: 500),
                    Version = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "timestamp"),
                })
                .PrimaryKey(t => t.ShopId);

            Sql("SET IDENTITY_INSERT dbo.Shops ON;");
            Sql("insert into Shops (ShopId, ShopName, ShopDescription, Version) values (1,'Test',NULL,NULL);");
            Sql("SET IDENTITY_INSERT dbo.Shops OFF;");

            AddColumn("dbo.Announcements", "ShopId", c => c.Int(nullable: false, defaultValueSql: "1"));
            AddColumn("dbo.Tickets", "ShopId", c => c.Int(nullable: false, defaultValueSql: "1"));
            CreateIndex("dbo.Tickets", "ShopId");
            AddForeignKey("dbo.Tickets", "ShopId", "dbo.Shops", "ShopId", cascadeDelete: true);
            DropColumn("dbo.Announcements", "ProjectId");
            DropColumn("dbo.Tickets", "ProjectId");
            DropTable("dbo.Projects");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Projects",
                c => new
                    {
                        ProjectId = c.Int(nullable: false, identity: true),
                        ProjectName = c.String(nullable: false, maxLength: 100),
                        ProjectDescription = c.String(maxLength: 500),
                        Version = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "timestamp"),
                    })
                .PrimaryKey(t => t.ProjectId);

            Sql("SET IDENTITY_INSERT dbo.Projects ON;");
            Sql("insert into Projects (ProjectId, ProjectName, ProjectDescription, Version) values (1,'Test',NULL,NULL);");
            Sql("SET IDENTITY_INSERT dbo.Projects OFF;");

            AddColumn("dbo.Tickets", "ProjectId", c => c.Int(nullable: false, defaultValueSql: "1"));
            AddColumn("dbo.Announcements", "ProjectId", c => c.Int(nullable: false, defaultValueSql: "1"));
            DropForeignKey("dbo.Tickets", "ShopId", "dbo.Shops");
            DropIndex("dbo.Tickets", new[] { "ShopId" });
            DropColumn("dbo.Tickets", "ShopId");
            DropColumn("dbo.Announcements", "ShopId");
            DropTable("dbo.Shops");
            CreateIndex("dbo.Tickets", "ProjectId");
            AddForeignKey("dbo.Tickets", "ProjectId", "dbo.Projects", "ProjectId", cascadeDelete: true);
        }
    }
}
