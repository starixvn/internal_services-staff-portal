namespace TicketDesk.Web.Client.Models
{
    using System.Collections.Generic;
    using Extensions;

    public class NavbarViewModel
    {
        public IList<SspMenuItem> MenuItems { get; set; }

        public bool IsAuthenticated { get; set; }

        public string Username { get; set; }
    }
}