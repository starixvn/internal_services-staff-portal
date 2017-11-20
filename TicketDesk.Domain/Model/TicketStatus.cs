namespace TicketDesk.Domain.Model
{
    using System.ComponentModel.DataAnnotations;
    using TicketDesk.Localization.Domain;

    public enum TicketStatus
    {
        [Display(Name = "TicketStatusActive", ResourceType = typeof(Strings))]
        Active,

        [Display(Name = "TicketStatusMoreInfo", ResourceType = typeof(Strings))]
        MoreInfo,

        [Display(Name = "TicketStatusResolved", ResourceType = typeof(Strings))]
        Resolved,

        [Display(Name = "TicketStatusClosed", ResourceType = typeof(Strings))]
        Closed
    }
}