namespace TicketDesk.Domain.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class Nexcel_Reference : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.References",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DateAppear = c.DateTime(nullable: false),
                        ImageLink = c.String(maxLength: 500, nullable: false),
                        CreatedBy = c.String(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        LastUpdateby = c.String(),
                        LastUpdateDate = c.DateTime(),
                        ReferenceType = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.References");
        }
    }
}
