namespace TicketDesk.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class Nexcel_add_CustomField_TicketCategory : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TicketCategoryCustomFields",
                c => new
                {
                    TicketCategory_CategoryId = c.Int(nullable: false),
                    CustomField_Id = c.Int(nullable: false),
                })
                .ForeignKey("dbo.TicketCategories", t => t.TicketCategory_CategoryId, cascadeDelete: true)
                .ForeignKey("dbo.CustomFields", t => t.CustomField_Id, cascadeDelete: true)
                .Index(t => t.TicketCategory_CategoryId)
                .Index(t => t.CustomField_Id);

        }

        public override void Down()
        {
            DropForeignKey("dbo.TicketCategoryCustomFields", "CustomField_Id", "dbo.CustomFields");
            DropForeignKey("dbo.TicketCategoryCustomFields", "TicketCategory_CategoryId", "dbo.TicketCategories");
            DropIndex("dbo.TicketCategoryCustomFields", new[] { "CustomField_Id" });
            DropIndex("dbo.TicketCategoryCustomFields", new[] { "TicketCategory_CategoryId" });
            DropTable("dbo.TicketCategoryCustomFields");
        }
    }
}
