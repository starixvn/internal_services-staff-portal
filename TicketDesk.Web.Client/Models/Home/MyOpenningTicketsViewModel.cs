namespace TicketDesk.Web.Client.Models.Home
{
    using System.Collections.Generic;
    using Domain.Model;

    public class MyOpenningTicketsViewModel
    {
        public MyOpenningTicketsViewModel()
        {
            TicketGroups = new List<TicketGroup>();
        }

        public IList<TicketGroup> TicketGroups { get; set; }
    }
}