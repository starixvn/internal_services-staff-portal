namespace TicketDesk.Domain.Model
{
    using System.ComponentModel.DataAnnotations;

    public class CustomFieldType
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }
    }
}