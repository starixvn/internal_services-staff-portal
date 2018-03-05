using System.ComponentModel.DataAnnotations;
using TicketDesk.Core.Enumerations;

namespace TicketDesk.Web.Client.Areas.Admin.Models.ShopManagement
{
    public class ShopFilterViewModel
    {
        [Display(Name = "Shop Name")]
        [StringLength(100)]
        public string ShopName { get; set; }

        public int ShopId { get; set; }

        [Display(Name = "Active")]
        public ActiveState IsActive { get; set; }
    }
}