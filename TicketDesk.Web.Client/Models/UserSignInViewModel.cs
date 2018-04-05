namespace TicketDesk.Web.Client.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;
    using Localization.Client;
    using Localization.Models;

    public class UserSignInViewModel
    {
        [Required]
        [Display(ResourceType = typeof(Resources), Name = "Views_User_SignIn_Username")]
        [AllowHtml]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [AllowHtml]
        [Display(Name = "Password", ResourceType = typeof(Strings))]
        public string Password { get; set; }

        [Display(Name = "RememberMe", ResourceType = typeof(Strings))]
        public bool RememberMe { get; set; }
    }
}