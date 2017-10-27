namespace TicketDesk.Web.Client.Models
{
    using Localization.Models;
    using System.ComponentModel.DataAnnotations;

    public class UserSignInViewModel
    {
        ////[Required(ErrorMessageResourceName = "EmptyUsername", ErrorMessageResourceType = typeof(Validation))]
        [Required]
        [Display(Name = "Username")]
        public string Email { get; set; }

        ////[Required(ErrorMessageResourceName = "EmptyPassword", ErrorMessageResourceType = typeof(Validation))]
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password", ResourceType = typeof(Strings))]
        public string Password { get; set; }

        [Display(Name = "RememberMe", ResourceType = typeof(Strings))]
        public bool RememberMe { get; set; }
    }
}