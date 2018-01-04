namespace TicketDesk.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Nexcel_Reference_Annoucement : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Announcements",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Message = c.String(nullable: false, maxLength: 5000),
                        CreatedDateTime = c.DateTime(nullable: false),
                        PublishedDateTime = c.DateTime(nullable: false),
                        ProjectId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        State = c.Int(nullable: false),
                        LastModified = c.DateTime(nullable: false),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.References",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DateAppear = c.DateTime(nullable: false),
                        ImageLink = c.String(nullable: false, maxLength: 500),
                        LastUpdateBy = c.String(nullable: false),
                        LastUpdateDate = c.DateTime(nullable: false),
                        ReferenceType = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.References");
            DropTable("dbo.Announcements");
        }
    }
}
