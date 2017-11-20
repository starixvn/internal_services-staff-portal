namespace TicketDesk.Web.Identity
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Model;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Security.Principal;

    public static class TdIdentityExtensions
    {
        public static IEnumerable<TicketDeskUser> GetUsersInRole(this ICollection<IdentityUserRole> roleUsers, UserManager<TicketDeskUser> userManager)
        {
            var ids = roleUsers.Select(u => u.UserId);
            return userManager.Users.Where(u => ids.Contains(u.Id));
        }

        public static string GetUserDisplayName(this IIdentity identity)
        {
            if (identity == null)
            {
                throw new ArgumentNullException(nameof(identity));
            }

            var ci = identity as ClaimsIdentity;
            return ci?.FindFirstValue(ClaimTypes.GivenName);
        }
    }
}