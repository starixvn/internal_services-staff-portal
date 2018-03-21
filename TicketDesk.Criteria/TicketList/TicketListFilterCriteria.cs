namespace TicketDesk.Criteria.TicketList
{
    using System;
    using Core.Enumerations;
    using Domain.Model;

    public class TicketListFilterCriteria
    {
        public int? ShopId { get; set; }

        public TicketCategoryTypes? Category { get; set; }

        public TicketStatus? Status { get; set; }

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }
    }
}