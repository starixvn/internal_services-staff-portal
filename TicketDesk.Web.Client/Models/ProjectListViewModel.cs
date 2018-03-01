using TicketDesk.Domain.Model;

namespace TicketDesk.Web.Client.Models
{
    public class ProjectListViewModelItem
    {
        public Shop Shop { get; set; }

        public int NumberOfTickets { get; set; }
    }
}