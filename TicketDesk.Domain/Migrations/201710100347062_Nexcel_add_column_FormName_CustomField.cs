namespace TicketDesk.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Nexcel_add_column_FormName_CustomField : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CustomFields", "FormName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.CustomFields", "FormName");
        }
    }
}
