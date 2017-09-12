using Fanex.NAP.Core.Criteria.User;
using Fanex.NAP.Core.Service.Impl;
using Fanex.NAP.Core.Web.Auth;
using Fanex.NAP.Core.Web.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace TicketDesk.Web.Client.App_Start
{
    public static class AuthConfig
    {
        public static void RegisterAuth(HttpContext context)
        {
            var user = HttpContext.Current.User;

            if (user != null)
            {
                if (user.Identity is FormsIdentity && user.Identity.IsAuthenticated)
                {
                    HttpCookie httpCookie = context.Request.Cookies[FormsAuthentication.FormsCookieName];

                    if (httpCookie != null)
                    {
                        FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(httpCookie.Value);

                        if (ticket == null || ticket.Expired || string.IsNullOrEmpty(ticket.UserData))
                        {
                            return;
                        }

                        var identity = new FormsIdentity(ticket);
                        AuthInfo udata = AuthInfo.CreateUserData(ticket.UserData);

                        context.User = new NapPrincipal(identity, udata);
                    }
                }
            }
        }

        public static void SignOut()
        {
            var currentSession = new CurrentSession();
            var userService = new UserService();
            var logOutUserCriteria = new LogOutUserCriteria();

            logOutUserCriteria.OnUserId = currentSession.OnUserId;
            userService.UserLogOut(logOutUserCriteria);
            currentSession.ClearAll();
        }
    }
}