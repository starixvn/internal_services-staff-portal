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

        public int? GroupId { get; set; }

        public string Value { get; set; }

        public int TicketId { get; set; }

        public int CustomFieldId { get; set; }

        //public virtual Ticket Ticket { get; set; }

        //public virtual CustomField CustomField { get; set; }
    }
}