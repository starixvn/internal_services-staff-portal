namespace TicketDesk.Web.Client.Models
{
    using System.ComponentModel.DataAnnotations;
    using TicketDesk.Localization;
    using TicketDesk.Localization.Models;

    public class UserSignInViewModel
    {
        private string dm = "asdadada";
        [Required(ErrorMessageResourceName = "ErrorEmptyUsername", ErrorMessageResourceType = typeof(Validation))]
        [Display(Name = "Username")]
        public string Email { get; set; }

        [Required(ErrorMessageResourceName = "ErrorEmptyPassword", ErrorMessageResourceType = typeof(Validation))]
        [DataType(DataType.Password)]
        [Display(Name = "Password", ResourceType = typeof(Strings))]
        public string Password { get; set; }

        [Display(Name = "RememberMe", ResourceType = typeof(Strings))]
        public bool RememberMe { get; set; }
    }
}