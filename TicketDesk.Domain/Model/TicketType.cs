namespace TicketDesk.Domain.Model
{
    using System.ComponentModel.DataAnnotations;

    public class TicketType
    {
        [Key]
        public int TicketTypeId { get; set; }

        public string TicketTypeName { get; set; }
    }
}
