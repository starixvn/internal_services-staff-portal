namespace TicketDesk.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class Nexcel_CustomField_MinMax_StringLength : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CustomFields", "MinStringLength", c => c.Int(nullable: false, defaultValue: -1));
            AddColumn("dbo.CustomFields", "MaxStringLength", c => c.Int(nullable: false, defaultValue: -1));
        }

        public override void Down()
        {
            DropColumn("dbo.CustomFields", "MaxStringLength");
            DropColumn("dbo.CustomFields", "MinStringLength");
        }
    }
}