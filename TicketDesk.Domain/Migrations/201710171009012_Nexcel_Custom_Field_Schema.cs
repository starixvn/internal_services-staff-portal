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
                .PrimaryKey(t => t.Id);
            
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
                        GroupId = c.String(),
                        Value = c.String(),
                        TicketId = c.Int(nullable: false),
                        CustomFieldId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
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
                        Id = c.Int(nullable: false, identity: true),
                        CategoryId = c.Int(nullable: false),
                        CustomFieldId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Tickets", "CategoryId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Tickets", "CategoryId");
            DropTable("dbo.TicketCategoryCustomFields");
            DropTable("dbo.TicketCategories");
            DropTable("dbo.CustomFieldValues");
            DropTable("dbo.CustomFieldTypes");
            DropTable("dbo.CustomFields");
        }
    }
}
