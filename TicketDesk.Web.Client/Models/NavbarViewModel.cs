namespace TicketDesk.Web.Client.Models
{
    using Fanex.NAP.Core.Model;
    using System.Collections.Generic;

    public class NavbarViewModel
    {
        public IList<MenuItem> MenuItems { get; set; }

        public bool IsAuthenticated { get; set; }

        public string Username { get; set; }
    }
}