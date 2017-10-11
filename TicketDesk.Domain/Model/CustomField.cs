namespace TicketDesk.Domain.Model
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public class CustomField
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public int CustomFieldTypeId { get; set; }

        public string Values { get; set; }

        [NotMapped]
        public CustomFieldType CustomFieldType { get; set; }

        [NotMapped]
        public ICollection<TicketCategory> TicketCategories { get; set; }

        [NotMapped]
        public ICollection<CustomFieldValue> CustomFieldValues { get; set; }

    }
}