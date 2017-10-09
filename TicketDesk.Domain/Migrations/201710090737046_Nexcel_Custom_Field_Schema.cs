namespace TicketDesk.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class Nexcel_Custom_Field_Schema : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CustomFields",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Name = c.String(),
                    CustomFieldTypeId = c.Int(nullable: false),
                    Values = c.String(),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CustomFieldTypes", t => t.CustomFieldTypeId, cascadeDelete: true)
                .Index(t => t.CustomFieldTypeId);

            CreateTable(
                "dbo.CustomFieldTypes",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Name = c.String(),
                })
                .PrimaryKey(t => t.Id);

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

            CreateTable(
                "dbo.TicketCategories",
                c => new
                {
                    CategoryId = c.Int(nullable: false, identity: true),
                    Name = c.String(),
                })
                .PrimaryKey(t => t.CategoryId);

            CreateTable(
                "dbo.TicketCategoryCustomFields",
                c => new
                {
                    TicketCategory_CategoryId = c.Int(nullable: false),
                    CustomField_Id = c.Int(nullable: false),
                })
                .PrimaryKey(t => new { t.TicketCategory_CategoryId, t.CustomField_Id })
                .ForeignKey("dbo.TicketCategories", t => t.TicketCategory_CategoryId, cascadeDelete: true)
                .ForeignKey("dbo.CustomFields", t => t.CustomField_Id, cascadeDelete: true)
                .Index(t => t.TicketCategory_CategoryId)
                .Index(t => t.CustomField_Id);

            AddColumn("dbo.Tickets", "CategoryId", c => c.Int());
            CreateIndex("dbo.Tickets", "CategoryId");
            AddForeignKey("dbo.Tickets", "CategoryId", "dbo.TicketCategories", "CategoryId");
        }

        public override void Down()
        {
            DropForeignKey("dbo.Tickets", "CategoryId", "dbo.TicketCategories");
            DropForeignKey("dbo.TicketCategoryCustomFields", "CustomField_Id", "dbo.CustomFields");
            DropForeignKey("dbo.TicketCategoryCustomFields", "TicketCategory_CategoryId", "dbo.TicketCategories");
            DropForeignKey("dbo.CustomFieldValues", "Ticket_TicketId", "dbo.Tickets");
            DropForeignKey("dbo.CustomFieldValues", "CustomField_Id", "dbo.CustomFields");
            DropForeignKey("dbo.CustomFields", "CustomFieldTypeId", "dbo.CustomFieldTypes");
            DropIndex("dbo.TicketCategoryCustomFields", new[] { "CustomField_Id" });
            DropIndex("dbo.TicketCategoryCustomFields", new[] { "TicketCategory_CategoryId" });
            DropIndex("dbo.Tickets", new[] { "CategoryId" });
            DropIndex("dbo.CustomFieldValues", new[] { "Ticket_TicketId" });
            DropIndex("dbo.CustomFieldValues", new[] { "CustomField_Id" });
            DropIndex("dbo.CustomFields", new[] { "CustomFieldTypeId" });
            DropColumn("dbo.Tickets", "CategoryId");
            DropTable("dbo.TicketCategoryCustomFields");
            DropTable("dbo.TicketCategories");
            DropTable("dbo.CustomFieldValues");
            DropTable("dbo.CustomFieldTypes");
            DropTable("dbo.CustomFields");
        }
    }
}
