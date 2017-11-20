namespace TicketDesk.Web.Identity
{
    using Localization.Identity;
    using Microsoft.AspNet.Identity;
    using Model;
    using System.Collections.Generic;
    using System.Linq;

    public class TicketDeskRoleManager : RoleManager<TicketDeskRole>
    {
        public TicketDeskRoleManager(IRoleStore<TicketDeskRole, string> roleStore)
            : base(roleStore)
        {
        }

        public static IEnumerable<TicketDeskRole> DefaultRoles => new[]
        {
            new TicketDeskRole
            {
                Name = "TdAdministrators",
                DisplayName = DefaultRolesDisplayName["TdAdministrators"],
                Description = DefaultRolesDescription["TdAdministrators"],
            },
            new TicketDeskRole
            {
                Name = "TdHelpDeskUsers",
                DisplayName = DefaultRolesDisplayName["TdHelpDeskUsers"],
                Description = DefaultRolesDescription["TdHelpDeskUsers"],
            },
            new TicketDeskRole
            {
                Name = "TdInternalUsers",
                DisplayName = DefaultRolesDisplayName["TdInternalUsers"],
                Description = DefaultRolesDescription["TdInternalUsers"],
            },
            new TicketDeskRole
            {
                Name = "TdPendingUsers",
                DisplayName = DefaultRolesDisplayName["TdPendingUsers"],
                Description = DefaultRolesDescription["TdPendingUsers"],
            }
        };

        public static IDictionary<string, string> DefaultRolesDisplayName => new Dictionary<string, string>
        {
            {
                "TdAdministrators",
                Strings.Role_Administrator
            },
            {
                "TdHelpDeskUsers",
                Strings.Role_HelpDesk
            },
            {
                "TdInternalUsers",
                Strings.Role_InternalUser
            },
            {
                "TdPendingUsers",
                Strings.Role_PendingApproval
            }
        };

        public static IDictionary<string, string> DefaultRolesDescription => new Dictionary<string, string>
        {
            {
                "TdAdministrators",
                Strings.Role_Administrator_Description
            },
            {
                "TdHelpDeskUsers",
                Strings.Role_HelpDesk_Description
            },
            {
                "TdInternalUsers",
                Strings.Role_InternalUser_Description
            },
            {
                "TdPendingUsers",
                Strings.Role_PendingApproval_Description
            }
        };

        public IOrderedEnumerable<TicketDeskUser> GetTdInternalUsers(TicketDeskUserManager userManager)
        {
            return GetUsersInRole(new[] { "TdInternalUsers", "TdHelpDeskUsers", "TdAdministrators" }, userManager);
        }

        public IOrderedEnumerable<TicketDeskUser> GetTdTdAdministrators(TicketDeskUserManager userManager)
        {
            return GetUsersInRole(new[] { "TdAdministrators" }, userManager);
        }

        public IOrderedEnumerable<TicketDeskUser> GetTdHelpDeskUsers(TicketDeskUserManager userManager)
        {
            return GetUsersInRole(new[] { "TdHelpDeskUsers", "TdAdministrators" }, userManager);
        }

        public IOrderedEnumerable<TicketDeskUser> GetTdPendingUsers(TicketDeskUserManager userManager)
        {
            return GetUsersInRole(new[] { "TdPendingUsers" }, userManager);
        }

        public IOrderedEnumerable<TicketDeskUser> GetUsersInRole(string[] roleNames, TicketDeskUserManager userManager)
        {
            // NOTE : WILL IMPLEMENT follow function policy
            var foundUsers = new List<TicketDeskUser>();

            foundUsers.AddRange(userManager.NapContextHelper.UserService.GetBySite(userManager.NapContextHelper.SiteId).Select(u => new TicketDeskUser
            {
                Id = u.Id.ToString(),
                Email = u.Email,
                DisplayName = u.FirstName + " " + u.LastName
            }));

            return foundUsers.OrderBy(u => u.DisplayName);
        }

        /// <summary>
        /// Ensures the correct set of TD standard roles exist.
        /// </summary>
        public void EnsureDefaultRolesExist()
        {
            //// var roles = TdIdentityContext.DefaultRoles;
            foreach (var defaultRole in DefaultRoles)
            {
                if (!this.RoleExists(defaultRole.Name))
                {
                    this.Create(defaultRole);
                }
            }
        }
    }
}