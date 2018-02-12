namespace TicketDesk.Core.Enumerations
{
    using System.ComponentModel.DataAnnotations;

    public enum AnnouncementState
    {
        [Display(Name = "No")]
        Scheduled,

        [Display(Name = "Yes")]
        Published
    }
}