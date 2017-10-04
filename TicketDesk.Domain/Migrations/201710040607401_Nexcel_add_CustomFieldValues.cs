namespace TicketDesk.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Nexcel_add_CustomFieldValues : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CustomFieldValues",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        GroupId = c.Int(),
                        Value = c.String(),
                        CustomField_Id = c.Int(),
                        Ticket_TicketId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CustomFields", t => t.CustomField_Id)
                .ForeignKey("dbo.Tickets", t => t.Ticket_TicketId)
                .Index(t => t.CustomField_Id)
                .Index(t => t.Ticket_TicketId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CustomFieldValues", "Ticket_TicketId", "dbo.Tickets");
            DropForeignKey("dbo.CustomFieldValues", "CustomField_Id", "dbo.CustomFields");
            DropIndex("dbo.CustomFieldValues", new[] { "Ticket_TicketId" });
            DropIndex("dbo.CustomFieldValues", new[] { "CustomField_Id" });
            DropTable("dbo.CustomFieldValues");
        }
    }
}
