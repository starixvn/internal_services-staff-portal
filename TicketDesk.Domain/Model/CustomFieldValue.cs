namespace TicketDesk.Domain.Model
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class CustomFieldValue
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string GroupId { get; set; }
        
        public string Value { get; set; }

        public int TicketId { get; set; }

        public int CustomFieldId { get; set; }
        
        [NotMapped]
        public virtual CustomField CustomField { get; set; }
    }
}