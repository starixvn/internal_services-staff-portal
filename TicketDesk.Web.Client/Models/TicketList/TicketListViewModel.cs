namespace TicketDesk.Web.Client.Models.TicketList
{
    using System.ComponentModel.DataAnnotations;
    using Core.Enumerations;
    using Domain.Model;
    using Localization.Client;

    public class TicketListViewModel
    {
        [Display(Name = nameof(Resources.Domain_Model_Ticket_Status), ResourceType = typeof(Resources))]
        public TicketStatus? Status { get; set; }

        [Display(Name = nameof(Resources.TicketCategory), ResourceType = typeof(Resources))]
        public TicketCategoryTypes? TicketType { get; set; }
    }
}