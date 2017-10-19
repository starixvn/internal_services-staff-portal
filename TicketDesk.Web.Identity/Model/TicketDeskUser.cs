namespace TicketDesk.Web.Identity.Model
{
    using Localization;
    using Localization.Identity;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System.ComponentModel.DataAnnotations;
    using System.Security.Claims;
    using System.Threading.Tasks;

    public class TicketDeskUser : IdentityUser
    {
        [Required(ErrorMessageResourceName = "FieldRequired", ErrorMessageResourceType = typeof(Validation))]
        [StringLength(100, ErrorMessageResourceName = "FieldMaximumLength", ErrorMessageResourceType = typeof(Validation))]
        [Display(Name = "DisplayName", ResourceType = typeof(Strings))]
        public string DisplayName { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<TicketDeskUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie).ConfigureAwait(false);
            userIdentity.AddClaim(new Claim(ClaimTypes.GivenName, DisplayName));
            return userIdentity;
        }
    }
}