namespace TicketDesk.Criteria.TicketList
{
    using System;
    using Core.Enumerations;
    using Domain.Model;

    public class TicketViewModel
    {
        public int Id { get; set; }

        public int ShopId { get; set; }

        public string Subject { get; set; }

        public TicketCategoryTypes TicketType { get; set; }

        public string ShopName { get; set; }
        public TicketStatus Status { get; set; }

        public DateTimeOffset CreatedDate { get; set; }

        public DateTimeOffset UpdatedDate { get; set; }

        public bool IsHostPermissionOnTicketType { get; set; }
    }
}