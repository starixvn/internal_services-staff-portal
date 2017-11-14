namespace TicketDesk.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Nexcel_CustomField_IsRequired : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CustomFields", "IsRequired", c => c.Boolean(nullable: false, defaultValue: true));
        }
        
        public override void Down()
        {
            DropColumn("dbo.CustomFields", "IsRequired");
        }
    }
}
