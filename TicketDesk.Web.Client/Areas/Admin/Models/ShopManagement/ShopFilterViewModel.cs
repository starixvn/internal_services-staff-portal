using TicketDesk.Core.Enumerations;

namespace TicketDesk.Web.Client.Areas.Admin.Models.ShopManagement
{
    public class ShopFilterViewModel
    {
        public string ShopName { get; set; }

        public int ShopId { get; set; }

        public ActiveState IsActive { get; set; }
    }
}