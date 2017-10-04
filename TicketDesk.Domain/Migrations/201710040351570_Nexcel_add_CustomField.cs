namespace TicketDesk.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Nexcel_add_CustomField : DbMigration
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
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CustomFields", "CustomFieldTypeId", "dbo.CustomFieldTypes");
            DropIndex("dbo.CustomFields", new[] { "CustomFieldTypeId" });
            DropTable("dbo.CustomFields");
        }
    }
}
