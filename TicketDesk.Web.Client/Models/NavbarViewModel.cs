namespace TicketDesk.Web.Client.Models
{
    using System.Collections.Generic;

    public class NavbarViewModel
    {
        public IList<Extensions.MenuItem> MenuItems { get; set; }

        public bool IsAuthenticated { get; set; }

        public string Username { get; set; }
    }
}