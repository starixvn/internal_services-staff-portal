namespace TicketDesk.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Nexcel_CustomField_RegexValidationErrorMessage : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CustomFields", "RegexValidationErrorMessage", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.CustomFields", "RegexValidationErrorMessage");
        }
    }
}
