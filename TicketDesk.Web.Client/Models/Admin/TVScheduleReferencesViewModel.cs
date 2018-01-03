namespace TicketDesk.Web.Client.Models.Admin
{
    using System;
    using TicketDesk.Core.Enumerations;

    public class TVScheduleReferencesViewModel : BaseReference
    {
        public ReferenceType ReferenceType { get; } = ReferenceType.TvSchedule;

        public DateTime DateAppear { get; set; }
    }
}