namespace TicketDesk.Domain.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class Nexcel_CustomField_MinMaxLength : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CustomFields", "MinLength", c => c.Int(nullable: false, defaultValue: -1));
            AddColumn("dbo.CustomFields", "MaxLength", c => c.Int(nullable: false, defaultValue: -1));
        }
        
        public override void Down()
        {
            DropColumn("dbo.CustomFields", "MaxLength");
            DropColumn("dbo.CustomFields", "MinLength");
        }
    }
}
