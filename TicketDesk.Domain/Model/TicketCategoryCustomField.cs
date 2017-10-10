namespace TicketDesk.Domain.Model
{
    using System.ComponentModel.DataAnnotations;

    public class TicketCategoryCustomField
    {
        [Key]
        public int Id { get; set; }

        public int CategoryId { get; set; }

        public int CustomFieldId { get; set; }
    }
}