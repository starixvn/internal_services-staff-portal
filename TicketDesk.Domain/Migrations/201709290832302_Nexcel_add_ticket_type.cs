namespace TicketDesk.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Nexcel_add_ticket_type : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TicketTypes",
                c => new
                    {
                        TicketTypeId = c.Int(nullable: false, identity: true),
                        TicketTypeName = c.String(),
                    })
                .PrimaryKey(t => t.TicketTypeId);
            
            AddColumn("dbo.Tickets", "TicketTypeId", c => c.Int(nullable: true));
            CreateIndex("dbo.Tickets", "TicketTypeId");
            AddForeignKey("dbo.Tickets", "TicketTypeId", "dbo.TicketTypes", "TicketTypeId", cascadeDelete: true);
            DropColumn("dbo.Tickets", "TicketType");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Tickets", "TicketType", c => c.String(nullable: false, maxLength: 50));
            DropForeignKey("dbo.Tickets", "TicketTypeId", "dbo.TicketTypes");
            DropIndex("dbo.Tickets", new[] { "TicketTypeId" });
            DropColumn("dbo.Tickets", "TicketTypeId");
            DropTable("dbo.TicketTypes");
        }
    }
}
