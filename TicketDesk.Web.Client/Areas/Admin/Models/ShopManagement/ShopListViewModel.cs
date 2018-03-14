namespace TicketDesk.Web.Client.Areas.Admin.Models.ShopManagement
{
    using System.Collections.Generic;
    using Domain.Model;

    public class ShopListViewModel
    {
        public bool IsHostPermission { get; set; }

        public IEnumerable<Shop> Shops { get; set; }
    }
}