namespace TicketDesk.Web.Client.Controllers
{
    using System;
    using System.Configuration;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Mvc;
    using Identity;
    using Identity.Model;
    using Localization.Controllers;
    using Microsoft.AspNet.Identity;
    using Microsoft.Owin.Security;
    using Models;
    using PushNotifications;
    using PushNotifications.Model;

    [TdAuthorize]
    [RoutePrefix("account")]
    [Route("{action=manage}")]
    public class AccountController : Controller
    {
        public AccountController(
            TicketDeskUserManager userManager,
            TicketDeskSignInManager signInManager,
            TdPushNotificationContext notificationContext)
        {
            UserManager = userManager;
            SignInManager = signInManager;
            NotificationContext = notificationContext;
        }

        public enum AccountMessageId
        {
            ChangePasswordSuccess,
            ProfileSaveSuccess,
            Error
        }

        private TicketDeskUserManager UserManager { get; }

        private TicketDeskSignInManager SignInManager { get; }

        private TdPushNotificationContext NotificationContext { get; }

        private IAuthenticationManager AuthenticationManager => HttpContext.GetOwinContext().Authentication;

        [Route("manage")]
        public ActionResult Manage(AccountMessageId? message)
        {
            ViewBag.StatusMessage =
                message == AccountMessageId.ChangePasswordSuccess ? Strings.ChangePasswordSuccess
                : message == AccountMessageId.Error ? Strings.Error
                : message == AccountMessageId.ProfileSaveSuccess ? Strings.ProfileSaveSuccess
                : string.Empty;

            return View();
        }

        [Route("change-password")]
        public ActionResult ChangePassword()
        {
            return View();
        }

        [Route("change-password")]
        [HttpPost]
        public async Task<ActionResult> ChangePassword(AccountPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var demoMode =
                (ConfigurationManager.AppSettings["ticketdesk:DemoModeEnabled"] ?? "false").Equals("true",
                    StringComparison.InvariantCultureIgnoreCase);
            if (demoMode)
            {
                ModelState.AddModelError("Password", Strings.UnableToChangeDemoUser);
            }
            else
            {
                var result = await UserManager
                    .ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword)
                    .ConfigureAwait(false);
                if (result.Succeeded)
                {
                    var user = await UserManager.FindByIdAsync(User.Identity.GetUserId()).ConfigureAwait(false);
                    if (user != null)
                    {
                        await SignInAsync(user, false).ConfigureAwait(false);
                    }

                    return RedirectToAction("Manage", new { Message = AccountMessageId.ChangePasswordSuccess });
                }

                AddErrors(result);
            }

            return View(model);
        }

        [Route("edit-profile")]
        public ActionResult EditProfile()
        {
            var model = new AccountProfileViewModel
            {
                DisplayName = User.Identity.GetUserDisplayName(),
                Email = User.Identity.GetUserName()
            };

            return View(model);
        }

        [Route("edit-profile")]
        [HttpPost]
        public async Task<ActionResult> EditProfile(AccountProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId()).ConfigureAwait(false);
            var oldEmail = user.Email;

            var demoMode = (ConfigurationManager.AppSettings["ticketdesk:DemoModeEnabled"] ?? "false")
                .Equals("true", StringComparison.InvariantCultureIgnoreCase);
            if (demoMode && oldEmail.EndsWith("@example.com", StringComparison.InvariantCultureIgnoreCase))
            {
                ModelState.AddModelError("Email", Strings.UnableToChangeDemoUser);
            }
            else
            {
                user.UserName = model.Email;
                user.Email = model.Email;
                user.DisplayName = model.DisplayName;
                var result = await UserManager.UpdateAsync(user).ConfigureAwait(false);

                if (result.Succeeded)
                {
                    await ResetMailEmailDestination(user, oldEmail).ConfigureAwait(false);

                    AuthenticationManager.SignOut();
                    await SignInManager.SignInAsync(user, false, false).ConfigureAwait(false);

                    return RedirectToAction("Manage", new { Message = AccountMessageId.ProfileSaveSuccess });
                }
            }

            return View(model);
        }

        private async Task ResetMailEmailDestination(TicketDeskUser user, string oldEmail)
        {
            var noteSettings = await NotificationContext
                .SubscriberPushNotificationSettingsManager
                .GetSettingsForSubscriberAsync(user.Id)
                .ConfigureAwait(false);

            var dest = noteSettings.PushNotificationDestinations.FirstOrDefault(d =>
                d.DestinationType == "email" && d.DestinationAddress == oldEmail);

            if (dest == null)
            {
                dest = new PushNotificationDestination { SubscriberId = user.Id, DestinationType = "email" };
                noteSettings.PushNotificationDestinations.Add(dest);
            }

            dest.DestinationAddress = user.Email;
            dest.SubscriberName = user.DisplayName;
            await NotificationContext.SaveChangesAsync().ConfigureAwait(false);
        }

        private async Task SignInAsync(TicketDeskUser user, bool isPersistent)
        {
            AuthenticationManager.SignOut(
                DefaultAuthenticationTypes.ExternalCookie,
                DefaultAuthenticationTypes.TwoFactorCookie);

            AuthenticationManager.SignIn(
                new AuthenticationProperties
                {
                    IsPersistent = isPersistent
                },
                await user.GenerateUserIdentityAsync(UserManager).ConfigureAwait(false));
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }
        }
    }
}