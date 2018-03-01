namespace TicketDesk.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Nexcel_AddCustomFieldListValue : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CustomFieldListValues",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CustomFieldId = c.Int(nullable: false),
                        DisplayName = c.String(),
                        DisplayName_vi = c.String(),
                        Description = c.String(),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.CustomFields", "CustomFieldListValueId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.CustomFields", "CustomFieldListValueId");
            DropTable("dbo.CustomFieldListValues");
        }
    }
}
