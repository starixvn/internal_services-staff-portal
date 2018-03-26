namespace TicketDesk.Web.Client.Models.TicketList
{
    using Core.Enumerations;
    using Domain.Model;

    public class TicketListViewModel
    {
        public TicketStatus? Status { get; set; }

        public TicketCategoryTypes? TicketType { get; set; }
    }
}