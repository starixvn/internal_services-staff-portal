namespace TicketDesk.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class nexcel_add_category : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TicketCategories",
                c => new
                    {
                        CategoryId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.CategoryId);

            AddColumn("dbo.Tickets", "CategoryId", c => c.Int(nullable: true, defaultValue: 1));
            CreateIndex("dbo.Tickets", "CategoryId");
            AddForeignKey("dbo.Tickets", "CategoryId", "dbo.TicketCategories", "CategoryId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Tickets", "CategoryId", "dbo.TicketCategories");
            DropIndex("dbo.Tickets", new[] { "CategoryId" });
            DropColumn("dbo.Tickets", "CategoryId");
            DropTable("dbo.TicketCategories");
        }
    }
}
