namespace TicketDesk.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Nexcel_UpdateShop : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Shops", "IsActive", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Shops", "IsActive");
        }
    }
}
