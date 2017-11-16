namespace TicketDesk.Domain.Model
{
    using Extensions.Validation;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class CustomFieldValue
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string GroupId { get; set; }
        
        [RequiredIf("IsRequired", true, ErrorMessage = "Custom Field value is required")]
        public string Value { get; set; }

        public int TicketId { get; set; }

        public int CustomFieldId { get; set; }

        [NotMapped]
        public bool IsRequired => CustomField.IsRequired;
        
        [NotMapped]
        public virtual CustomField CustomField { get; set; }
    }
}