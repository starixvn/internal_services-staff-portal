namespace TicketDesk.Domain.Model
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class TicketCategory
    {
        [Key]
        public int CategoryId { get; set; }

        public string Name { get; set; }

        public virtual ICollection<CustomField> CustomFields { get; set; }

        public virtual ICollection<Ticket> Tickets { get; set; }
    }
}