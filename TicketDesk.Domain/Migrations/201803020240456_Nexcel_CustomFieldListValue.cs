namespace TicketDesk.Domain.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class Nexcel_CustomFieldListValue : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                    "dbo.CustomFieldListValues",
                    c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CustomFieldId = c.Int(nullable: false),
                        DisplayName = c.String(maxLength: 50),
                        DisplayName_vi = c.String(nullable: false, maxLength: 50),
                        Description = c.String(maxLength: 200),
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
