// TicketDesk - Attribution notice
// Contributor(s):
//
//      Stephen Redd (https://github.com/stephenredd)
//
// This file is distributed under the terms of the Microsoft Public
// License (Ms-PL). See http://opensource.org/licenses/MS-PL
// for the complete terms of use.
//
// For any distribution that contains code from this file, this notice of
// attribution must remain intact, and a copy of the license must be
// provided to the recipient.

using Fanex.Common;
using Fanex.NAP.Core.Criteria.User;
using Fanex.NAP.Core.Model;
using Fanex.NAP.Core.Service.Interface;
using Fanex.NAP.Core.Web;
using Fanex.NAP.Core.Web.Auth;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Security;
using TicketDesk.Domain;
using TicketDesk.PushNotifications;
using TicketDesk.PushNotifications.Model;
using TicketDesk.Web.Client.Models;
using TicketDesk.Web.Identity;
using TicketDesk.Web.Identity.Model;

namespace TicketDesk.Web.Client.Controllers
{
    [RoutePrefix("user")]
    [Route("{action}")]
    public class UserController : BaseController
    {
        private TicketDeskUserManager UserManager { get; set; }
        private TicketDeskSignInManager SignInManager { get; set; }
        private TdDomainContext DomainContext { get; set; }

        private readonly IUserService userService;
        private readonly IGroupService groupService;
        private readonly ISiteService siteService;
        private readonly IMenuItemService menuItemService;

        public UserController(
            TicketDeskUserManager userManager,
            TicketDeskSignInManager signInManager,
            TdDomainContext domainContext,
            IUserService userService,
            IGroupService groupService,
            ISiteService siteService,
            IMenuItemService menuItemService)
        {
            UserManager = userManager;
            SignInManager = signInManager;
            DomainContext = domainContext;
            this.userService = userService;
            this.groupService = groupService;
            this.siteService = siteService;
            this.menuItemService = menuItemService;
        }

        [AllowAnonymous]
        [Route("register")]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [Route("register")]
        public async Task<ActionResult> Register(UserRegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new TicketDeskUser { UserName = model.Email, Email = model.Email, DisplayName = model.DisplayName };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await UserManager.AddToRolesAsync(user.Id, DomainContext.TicketDeskSettings.SecuritySettings.DefaultNewUserRoles.ToArray());
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                    HostingEnvironment.QueueBackgroundWorkItem(ct =>
                    {
                        using (var notificationContext = new TdPushNotificationContext())
                        {
                            notificationContext.SubscriberPushNotificationSettingsManager.AddSettingsForSubscriber(
                                new SubscriberNotificationSetting
                                {
                                    SubscriberId = user.Id,
                                    IsEnabled = true,
                                    PushNotificationDestinations = new[]
                                    {
                                        new PushNotificationDestination()
                                        {
                                            DestinationType = "email",
                                            DestinationAddress = user.Email,
                                            SubscriberName = user.DisplayName
                                        }
                                    }
                                });
                            notificationContext.SaveChanges();
                        }
                    });

                    return RedirectToAction("Index", "Home");
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [AllowAnonymous]
        [Route("sign-in")]
        public ActionResult SignIn(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            ViewBag.IsDemoMode = (ConfigurationManager.AppSettings["ticketdesk:DemoModeEnabled"] ?? "false").Equals("true", StringComparison.InvariantCultureIgnoreCase);

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [Route("sign-in")]
        public async Task<ActionResult> SignIn(UserSignInViewModel model, string returnUrl)
        {
            ViewBag.IsDemoMode = (ConfigurationManager.AppSettings["ticketdesk:DemoModeEnabled"] ?? "false").Equals("true", StringComparison.InvariantCultureIgnoreCase);

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = userService.GetByUsername(model.Email) as User;

            if (user != null)
            {
                var password = Cryptography.CfsEnCode(model.Password);

                if (password == user.Password)
                {
                    var siteLevelList = siteService.GetSiteLevelByUser(user.Id, "UAT");

                    if (siteLevelList.Any(x => x.Site.Id == SiteId))
                    {
                        var group = user.Group == null ? null : groupService.GetById(user.Group.Id);

                        if ((group == null || group.IsActive) && siteLevelList.Any(x => x.Site.Id == SiteId && x.IsActive))
                        {
                            // check-in user
                            var criteria = new UserCheckInUserCriteria
                            {
                                UserId = user.Id,
                                SiteId = SiteId,
                                NetSessionId = Session.SessionID,
                                Ip = CurrentContext.IP,
                                LoginDate = DateTime.Now,
                                BrowserName = CurrentContext.BrowserName,
                                CountryCode = CurrentContext.CountryCode,
                                Domain = CurrentContext.DomainName,
                                Platform = CurrentContext.Platform
                            };

                            userService.UserCheckIn(criteria);

                            int level = siteLevelList.FirstOrDefault(x => x.Site.Id == SiteId).Level;

                            SaveUserSession(user, criteria.OnUserId, level);

                            CreateUserMenu(user.Id, Environment, SiteId);
                            CreateAuthInfo(user);

                            //SignInManager.PasswordSignInAsync(model.Email, "123456aa", model.RememberMe, true);

                            return RedirectToLocal(returnUrl);
                            // return Redirect(FormsAuthentication.DefaultUrl);
                        }
                        else
                        {
                            ViewBag.Message = ViewResources.Your_account_was_suspended;
                            ModelState.AddModelError("", ViewResources.Your_account_was_suspended);
                        }
                    }
                    else
                    {
                        ViewBag.Message = ViewResources.You_have_no_authorization;
                        ModelState.AddModelError("", ViewResources.You_have_no_authorization);
                    }
                }
                else
                {
                    // wrong password
                    ViewBag.Message = ViewResources.Invalid_Username_or_Password;
                    ModelState.AddModelError("", ViewResources.Invalid_Username_or_Password);
                }
            }
            else
            {
                // user doesn't exist
                ViewBag.Message = ViewResources.Invalid_Username_or_Password;
                ModelState.AddModelError("", ViewResources.Invalid_Username_or_Password);
            }

            return View(model);

            //var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, true);
            //switch (result)
            //{
            //    case SignInStatus.Success:
            //        return RedirectToLocal(returnUrl);
            //    case SignInStatus.LockedOut:
            //        return View("Lockout");
            //    default:
            //        ModelState.AddModelError("", Strings.InvalidLoginAttempt);
            //        return View(model);
            //}
        }

        private void CreateAuthInfo(User user)
        {
            var currentDomain = CurrentContext.CurrentDomain;

            var authInfo = new AuthInfo();

            authInfo.UserId = user.Id;
            authInfo.OnUserId = 1;

            var authTicket = new FormsAuthenticationTicket(
                 1,
                user.Username,
                DateTime.Now,
                DateTime.Now.AddMinutes(20000),
                false,
                authInfo.ToXml(),
                currentDomain.Path);

            string encryptedTicket = FormsAuthentication.Encrypt(authTicket);
            var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket)
            {
                HttpOnly = true,
                Path = currentDomain.Path,
                Name = FormsAuthentication.FormsCookieName
            };

            Response.Cookies.Add(authCookie);
        }

        [Route("sign-out")]
        public ActionResult SignOut()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("Index", "Home");
        }

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        private void CreateUserMenu(int userId, string environment, int siteId)
        {
            CurrentSession.MenuItemList = (IList<MenuItem>)menuItemService.GetByUserAndSiteIncludeGroup(
                userId,
                siteId,
                environment,
                true);
        }
    }
}