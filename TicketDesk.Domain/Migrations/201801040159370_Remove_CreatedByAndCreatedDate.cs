namespace TicketDesk.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Remove_CreatedByAndCreatedDate : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.References", "ImageLink", c => c.String(maxLength: 500));
            DropColumn("dbo.References", "CreatedBy");
            DropColumn("dbo.References", "CreatedDate");
        }
        
        public override void Down()
        {
            AddColumn("dbo.References", "CreatedDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.References", "CreatedBy", c => c.String());
            AlterColumn("dbo.References", "ImageLink", c => c.String());
        }
    }
}
