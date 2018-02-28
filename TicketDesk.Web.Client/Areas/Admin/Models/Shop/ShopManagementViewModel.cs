using TicketDesk.Core.Enumerations;

namespace TicketDesk.Web.Client.Areas.Admin.Models.Shop
{
    public class ShopManagementViewModel
    {
        public string ShopName { get; set; }

        public int ShopId { get; set; }

        public ActiveState IsActive { get; set; }
    }
}