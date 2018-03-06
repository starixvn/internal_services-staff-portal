namespace TicketDesk.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Nexcel_CustomFieldIsAllowToEditListValue : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CustomFields", "IsAllowedToEditListValue", c => c.Boolean(nullable: false, defaultValueSql:"0"));
        }
        
        public override void Down()
        {
            DropColumn("dbo.CustomFields", "IsAllowedToEditListValue");
        }
    }
}
