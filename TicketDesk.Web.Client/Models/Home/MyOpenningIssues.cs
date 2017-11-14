namespace TicketDesk.Web.Client.Models.Home
{
    using System.Collections.Generic;

    public class MyOpenningIssues : Pagination
    {
        public IList<TicketGroup> TicketGroups { get; set; }
    }
}