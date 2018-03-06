namespace TicketDesk.Domain.Model
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Web.Mvc;
    using TicketDesk.Localization.Client;

    public class CustomFieldListValue
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int CustomFieldId { get; set; }

        [AllowHtml]
        [Display(Name = nameof(Resources.DisplayNameEN), ResourceType = typeof(Resources))]
        [StringLength(
            50,
            ErrorMessageResourceType = typeof(Resources),
            ErrorMessageResourceName = nameof(Resources.StringLengthMaxErrorMessage))]
        public string DisplayName { get; set; }

        [AllowHtml]
        [Display(Name = nameof(Resources.DisplayNameVI), ResourceType = typeof(Resources))]
        [StringLength(
            50,
            ErrorMessageResourceType = typeof(Resources),
            ErrorMessageResourceName = nameof(Resources.StringLengthMaxErrorMessage))]
        [Required(
            ErrorMessageResourceType = typeof(Resources),
            ErrorMessageResourceName = nameof(Resources.RequiredErrorMessage))]
        public string DisplayName_vi { get; set; }

        [AllowHtml]
        [Display(Name = nameof(Resources.Description), ResourceType = typeof(Resources))]
        [StringLength(
            200,
            ErrorMessageResourceType = typeof(Resources),
            ErrorMessageResourceName = nameof(Resources.StringLengthMaxErrorMessage))]
        public string Description { get; set; }

        [Display(Name = nameof(Resources.Active), ResourceType = typeof(Resources))]
        public bool IsActive { get; set; }
    }
}