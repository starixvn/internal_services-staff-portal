namespace TicketDesk.Domain.Model
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class CustomFieldType
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        //public virtual ICollection<CustomField> CustomFields { get; set; }
    }
}