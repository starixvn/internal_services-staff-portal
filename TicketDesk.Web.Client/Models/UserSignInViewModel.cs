namespace TicketDesk.Web.Client.Models
{
    using Localization.Client;
    using Localization.Models;
    using System.ComponentModel.DataAnnotations;

    public class UserSignInViewModel
    {
        [Required]
        [Display(ResourceType = typeof(Resources), Name = "Views_User_SignIn_Username")]
        public string Email { get; set; }
        
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password", ResourceType = typeof(Strings))]
        public string Password { get; set; }

        [Display(Name = "RememberMe", ResourceType = typeof(Strings))]
        public bool RememberMe { get; set; }
    }
}