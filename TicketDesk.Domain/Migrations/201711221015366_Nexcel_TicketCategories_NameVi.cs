namespace TicketDesk.Domain.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class Nexcel_TicketCategories_NameVi : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TicketCategories", "Name_vi", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.TicketCategories", "Name_vi");
        }
    }
}
