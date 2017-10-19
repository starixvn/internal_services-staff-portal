namespace TicketDesk.Domain.Model
{
    using System.ComponentModel.DataAnnotations;

    public class TicketCategory
    {
        [Key]
        public int CategoryId { get; set; }

        public string Name { get; set; }
    }
}