namespace TicketDesk.Domain.Model
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class EditableField
    {
        public string Name { get; set; }

        public int CustomFieldId { get; set; }
    }

    public class CustomFieldListValue
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int CustomFieldId { get; set; }

        [Display(Name = "Display Name EN")]
        [StringLength(50,ErrorMessage = "{0} must be less than {1} characters")]
        public string DisplayName { get; set; }

        [Display(Name = "Display Name VI")]
        [StringLength(50, ErrorMessage = "{0} must be less than {1} characters")]
        [Required(ErrorMessage = "{0} is required!")]
        public string DisplayName_vi { get; set; }

        [Display(Name = "Description")]
        [StringLength(200, ErrorMessage = "{0} must be less than {1} characters")]
        public string Description { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; }
    }
}
