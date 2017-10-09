namespace TicketDesk.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Nexcel_add_foreign_key_Ticket_TicketCategory : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Tickets", "CategoryId", "dbo.TicketCategories");
            DropIndex("dbo.Tickets", new[] { "CategoryId" });
            AlterColumn("dbo.Tickets", "CategoryId", c => c.Int(nullable: false));
            CreateIndex("dbo.Tickets", "CategoryId");
            AddForeignKey("dbo.Tickets", "CategoryId", "dbo.TicketCategories", "CategoryId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Tickets", "CategoryId", "dbo.TicketCategories");
            DropIndex("dbo.Tickets", new[] { "CategoryId" });
            AlterColumn("dbo.Tickets", "CategoryId", c => c.Int());
            CreateIndex("dbo.Tickets", "CategoryId");
            AddForeignKey("dbo.Tickets", "CategoryId", "dbo.TicketCategories", "CategoryId");
        }
    }
}
