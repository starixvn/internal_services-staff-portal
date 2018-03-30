namespace TicketDesk.Domain.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class Nexcel_UploadedUserId : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UploadedUserIds",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CustomerUsername = c.String(),
                        CustomerName = c.String(),
                        IdNumber = c.String(),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.UploadedUserIds");
        }
    }
}
