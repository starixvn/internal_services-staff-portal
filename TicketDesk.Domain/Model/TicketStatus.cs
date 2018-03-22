namespace TicketDesk.Domain.Model
{
    using System.ComponentModel.DataAnnotations;
    using TicketDesk.Localization.Domain;

    public enum TicketStatus
    {
        [Display(Name = nameof(Strings.TicketStatusNew), ResourceType = typeof(Strings))]
        New,

        [Display(Name = nameof(Strings.TicketStatusInProgress), ResourceType = typeof(Strings))]
        InProgress,

        [Display(Name = nameof(Strings.TicketStatusRejected), ResourceType = typeof(Strings))]
        Rejected,

        [Display(Name = nameof(Strings.TicketStatusPending), ResourceType = typeof(Strings))]
        Pending,

        [Display(Name = nameof(Strings.TicketStatusClosed), ResourceType = typeof(Strings))]
        Closed
    }
}