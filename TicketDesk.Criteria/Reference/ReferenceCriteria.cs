namespace TicketDesk.Criteria.Reference
{
    using System;
    using Core.Enumerations;

    public class ReferenceCriteria
    {
        public int Id { get; set; }

        public DateTime DateAppear { get; set; }

        public string ImageLink { get; set; }

        public ReferenceType ReferenceType { get; set; }
    }
}