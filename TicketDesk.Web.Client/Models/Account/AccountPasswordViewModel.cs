namespace TicketDesk.Web.Client.Models
{
    using System.ComponentModel.DataAnnotations;
    using Localization;
    using Localization.Models;

    public class AccountPasswordViewModel
    {
        [Required(ErrorMessageResourceName = "FieldRequired", ErrorMessageResourceType = typeof(Validation))]
        [DataType(DataType.Password)]
        [Display(Name = "CurrentPassword", ResourceType = typeof(Strings))]
        public string OldPassword { get; set; }

        [Required(ErrorMessageResourceName = "FieldRequired", ErrorMessageResourceType = typeof(Validation))]
        [StringLength(100, ErrorMessageResourceName = "FieldMinimumLength", ErrorMessageResourceType = typeof(Validation), MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "NewPassword", ResourceType = typeof(Strings))]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "ConfirmNewPassword", ResourceType = typeof(Strings))]
        [Compare("NewPassword", ErrorMessageResourceName = "NewConfirmationDoNotMatch", ErrorMessageResourceType = typeof(Strings))]
        public string ConfirmPassword { get; set; }
    }
}