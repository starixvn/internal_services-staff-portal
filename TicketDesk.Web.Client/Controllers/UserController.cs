namespace TicketDesk.Web.Client.Controllers
{
    using Fanex.Common;
    using Fanex.NAP.Core.Criteria.User;
    using Fanex.NAP.Core.Model;
    using Fanex.NAP.Core.Service.Interface;
    using Fanex.NAP.Core.Web;
    using Fanex.NAP.Core.Web.Auth;
    using Localization;
    using Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Security;

    [RoutePrefix("user")]
    [Route("{action}")]
    public class UserController : BaseController
    {
        private readonly IGroupService _groupService;
        private readonly ISiteService _siteService;
        private readonly IMenuItemService _menuItemService;

        public UserController(
            IGroupService groupService,
            ISiteService siteService,
            IMenuItemService menuItemService)
        {
            _groupService = groupService;
            _siteService = siteService;
            _menuItemService = menuItemService;
        }

        [AllowAnonymous]
        [Route("sign-in")]
        public ActionResult SignIn(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;

            if (Request.IsAuthenticated)
            {
                return string.IsNullOrEmpty(returnUrl) 
                    ? RedirectToLocal(returnUrl) 
                    : RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        [Route("sign-in")]
        public ActionResult SignIn(UserSignInViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            model.Email = HttpUtility.HtmlEncode(model.Email);

            var user = UserService.GetByUsername(model.Email) as User;

            if (user != null)
            {
                var password = Cryptography.CfsEnCode(model.Password);

                if (password == user.Password)
                {
                    var siteLevelList = _siteService.GetSiteLevelByUser(user.Id, NapContextHelper.Environment);
                    var group = user.Group == null ? null : _groupService.GetById(user.Group.Id);

                    if (siteLevelList.Any(x => x.Site.Id == NapContextHelper.SiteId && x.IsActive) && (group == null || group.IsActive))
                    {
                        CheckInUser(user);
                        return RedirectToLocal(returnUrl);
                    }

                    ViewBag.Message = ViewResources.You_have_no_authorization;
                    ModelState.AddModelError(string.Empty, ViewResources.You_have_no_authorization);
                }
                else
                {
                    // wrong password
                    ViewBag.Message = ViewResources.Invalid_Username_or_Password;
                    ModelState.AddModelError(string.Empty, Validation.InvalidUsernameOrPassword);
                }
            }
            else
            {
                // user doesn't exist
                ViewBag.Message = ViewResources.Invalid_Username_or_Password;
                ModelState.AddModelError(string.Empty, Validation.InvalidUsernameOrPassword);
                ////ModelState.AddModelError(string.Empty, Validation.InvalidUsername);
            }

            return View(model);
        }

        private void CheckInUser(User user)
        {
            // check-in user
            DoCheckin(user);

            if (NapContextHelper.SiteId != 0)
            {
                CreateUserMenu(user.Id);
            }

            CreateAuthInfo(user);
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

            var authInfo = new AuthInfo
            {
                UserId = user.Id,
                OnUserId = CurrentSession.OnUserId
            };

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

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Home");
        }

        private void CreateUserMenu(int userId)
        {
            CurrentSession.MenuItemList = (IList<MenuItem>)_menuItemService.GetByUserAndSiteIncludeGroup(
                    userId,
                    NapContextHelper.SiteId,
                    NapContextHelper.Environment,
                    true);
        }

        private void DoCheckin(User user)
        {
            var criteria = new UserCheckInUserCriteria
            {
                UserId = user.Id,
                SiteId = NapContextHelper.SiteId,
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
            CurrentSession.Level = UserService.GetUserSiteLevel(user.Id, NapContextHelper.SiteId);
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

            DeleteCookiesAndClearSession();
        }

        private void DeleteCookiesAndClearSession()
        {
            Response.Cookies.Clear();

            HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];

            if (authCookie != null)
            {
                authCookie.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(authCookie);
            }

            CurrentSession.ClearAll();
            Session.Clear();
            Session.Abandon();
        }
    }
}