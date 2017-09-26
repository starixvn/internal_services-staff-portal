namespace TicketDesk.Web.Client.Models
{
    using System.ComponentModel.DataAnnotations;
    using TicketDesk.Localization;
    using TicketDesk.Localization.Models;

    public class UserSignInViewModel
    {
        [Required(ErrorMessageResourceName = "FieldRequired", ErrorMessageResourceType = typeof(Validation))]
        [Display(Name = "Username")]
        public string Email { get; set; }

        [Required(ErrorMessageResourceName = "FieldRequired", ErrorMessageResourceType = typeof(Validation))]
        [DataType(DataType.Password)]
        [Display(Name = "Password", ResourceType = typeof(Strings))]
        public string Password { get; set; }
    }
}