namespace TicketDesk.Domain.Model
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class CustomField
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public int CustomFieldTypeId { get; set; }

        public string Values { get; set; }

        //public virtual CustomFieldType CustomFieldType { get; set; }

        //public virtual ICollection<TicketCategory> TicketCategories { get; set; }

        //public virtual ICollection<CustomFieldValue> CustomFieldValues { get; set; }
    }
}