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
    using Fanex.NAP.Core.Domain;

    [RoutePrefix("user")]
    [Route("{action}")]
    public class UserController : BaseController
    {
        private TicketDeskUserManager UserManager { get; set; }
        private TicketDeskSignInManager SignInManager { get; set; }
        private TdDomainContext DomainContext { get; set; }

        private readonly IGroupService groupService;
        private readonly ISiteService siteService;
        private readonly IMenuItemService menuItemService;

        public UserController(
            TicketDeskUserManager userManager,
            TicketDeskSignInManager signInManager,
            TdDomainContext domainContext,
            IGroupService groupService,
            ISiteService siteService,
            IMenuItemService menuItemService)
        {
            UserManager = userManager;
            SignInManager = signInManager;
            DomainContext = domainContext;
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

            var user = UserService.GetByUsername(model.Email) as User;

            if (user != null)
            {
                var password = Cryptography.CfsEnCode(model.Password);

                if (password == user.Password)
                {
                    var siteLevelList = siteService.GetSiteLevelByUser(user.Id, Environment);
                    var group = user.Group == null ? null : groupService.GetById(user.Group.Id);

                    if (siteLevelList.Any(x => x.Site.Id == SiteId))
                    {
                        if ((group == null || group.IsActive) && siteLevelList.Any(x => x.Site.Id == SiteId && x.IsActive))
                        {
                            // check-in user
                            DoCheckin(user);

                            if (SiteId != 0)
                            {
                                CreateUserMenu(user.Id);
                            }

                            CreateAuthInfo(user);

                            return RedirectToLocal(returnUrl);
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
        }

        [Route("sign-out")]
        public ActionResult SignOut()
        {
            DoSignOut();
            return RedirectToAction("Index", "Home");
        }

        private void CreateAuthInfo(User user)
        {
            var currentDomain = CurrentContext.CurrentDomain;

            var authInfo = new AuthInfo();

            authInfo.UserId = user.Id;
            authInfo.OnUserId = CurrentSession.OnUserId;

            var authTicket = new FormsAuthenticationTicket(
                 1,
                user.Username,
                DateTime.Now,
                DateTime.Now.AddMinutes(FormsAuthentication.Timeout.TotalMinutes),
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

        /// <summary>
        /// Get Menus from NAP
        /// </summary>
        /// <param name="userId"></param>
        private void CreateUserMenu(int userId)
        {
            var menus = menuItemService.GetByUserAndSiteIncludeGroup(
                userId,
                SiteId,
                Environment,
                true);

            IList<IMenuItem> parentMenu = menus.Where(x => x.Parent == null).ToList();

            CurrentSession.MenuItemList = new List<MenuItem>();

            foreach (var item in parentMenu)
            {
                var concreteItem = (MenuItem)item;
                CurrentSession.MenuItemList.Add(concreteItem);
            }

            CurrentSession.MenuItemList = CurrentSession.MenuItemList.OrderBy(x => x.DisplayOrder).ToList();
        }

        private void DoCheckin(User user)
        {
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

            UserService.UserCheckIn(criteria);

            // saves user session
            CurrentSession.Level = UserService.GetUserSiteLevel(user.Id, SiteId);
            CurrentSession.UserId = user.Id;
            CurrentSession.Username = user.Username;
            CurrentSession.OnUserId = criteria.OnUserId;
            CurrentSession.SessionId = Session.SessionID;
        }

        private void DoSignOut()
        {
            var criteria = new LogOutUserCriteria
            {
                OnUserId = CurrentSession.OnUserId
            };
            UserService.UserLogOut(criteria);
            FormsAuthentication.SignOut();
            Response.Cookies.Clear();
            CurrentSession.ClearAll();
        }
    }
}