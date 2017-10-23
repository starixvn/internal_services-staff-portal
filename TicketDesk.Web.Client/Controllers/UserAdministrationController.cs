namespace TicketDesk.Web.Client.Controllers
{
    using Microsoft.AspNet.Identity;
    using PagedList;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Localization.Controllers;
    using Models;
    using Identity;
    using Identity.Model;

    [RoutePrefix("admin")]
    [Route("{action=index}")]
    [TdAuthorize(Roles = "TdAdministrators")]
    public class UserAdministrationController : Controller
    {
        public UserAdministrationController(TicketDeskUserManager userManager, TicketDeskRoleManager roleManager)
        {
            UserManager = userManager;
            RoleManager = roleManager;
        }

        private TicketDeskUserManager UserManager { get; }

        private TicketDeskRoleManager RoleManager { get; }

        [Route("users/{page:int?}")]
        public async Task<ActionResult> Index(int? page)
        {
            ViewBag.AllRolesList = await RoleManager.Roles.ToListAsync().ConfigureAwait(false);
            var users = await GetUsersForListAsync(page ?? 1).ConfigureAwait(false);
            return View(users);
        }

        [Route("users/page/{page:int?}")]
        public async Task<ActionResult> PageList(int? page = 1)
        {
            ViewBag.AllRolesList = await RoleManager.Roles.ToListAsync().ConfigureAwait(false);
            var users = await GetUsersForListAsync(page ?? 1).ConfigureAwait(false);
            return PartialView("_UserList", users);
        }

        [Route("user/{id?}")]
        public async Task<ActionResult> Edit(string id)
        {
            var user = await UserManager.FindByIdAsync(id).ConfigureAwait(false);
            if (user == null)
            {
                return RedirectToAction("Index");
            }

            var model = new UserAccountInfoViewModel(user, user.Roles.Select(r => r.RoleId));
            var pendingRole = RoleManager.Roles.FirstOrDefault(r => r.Name == "TdPendingUsers");

            ViewBag.TdPendingUsersRoleId = pendingRole == null ? string.Empty : pendingRole.Id;
            return View(model);
        }

        [Route("user/{id?}")]
        [HttpPost]
        public async Task<ActionResult> Edit(UserAccountInfoViewModel model)
        {
            var pendingRole = RoleManager.Roles.FirstOrDefault(r => r.Name == "TdPendingUsers");
            ViewBag.TdPendingUsersRoleId = pendingRole == null 
                ? string.Empty 
                : pendingRole.Id;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await UserManager.FindByIdAsync(model.User.Id).ConfigureAwait(false);
            var demoMode = (ConfigurationManager.AppSettings["ticketdesk:DemoModeEnabled"] ?? "false")
                .Equals("true", StringComparison.InvariantCultureIgnoreCase);

            if (demoMode && model.User.Email.EndsWith("@example.com", StringComparison.InvariantCultureIgnoreCase))
            {
                ModelState.AddModelError("Profile", Strings.UnableToChangeDemoUser);
            }
            else
            {
                // deal with locks first
                if (await SetLockout(model, user).ConfigureAwait(false) &&
                    await UpdateUserInfo(user, model).ConfigureAwait(false))
                {
                    RemovePendingRoles(model, pendingRole);

                    List<string> roleIdsToRemove = GetChangedRoles(model, user);

                    var roleIdsToAdd = model.Roles.Except(user.Roles.Select(ur => ur.RoleId)).ToList();
                    bool isRemoveSuccess = await RemoveRoles(user, roleIdsToRemove, roleIdsToAdd).ConfigureAwait(false);

                    if (isRemoveSuccess)
                    {
                        return RedirectToAction("Index");
                    }
                }
            }

            // Since the above operations could be partially committed before a failure,
            // we will re-get the user so we are sure the screen will accurately show
            // the current data. Model errors will have been added by the respective
            // update operations that failed
            user = await UserManager.FindByIdAsync(model.User.Id).ConfigureAwait(false);
            UserAccountInfoViewModel viewModel = new UserAccountInfoViewModel(user, user.Roles.Select(r => r.RoleId));
            return View(viewModel);
        }

        private async Task<bool> RemoveRoles(TicketDeskUser user, List<string> roleIdsToRemove, List<string> roleIdsToAdd)
        {
            // do role removes
            return await RemoveRoles(roleIdsToRemove, user).ConfigureAwait(false) &&
                                  await AddRoles(roleIdsToAdd, user).ConfigureAwait(false);
        }

        private List<string> GetChangedRoles(UserAccountInfoViewModel model, TicketDeskUser user)
        {
            // get role changes
            var roleIdsToRemove = user.Roles.Select(ur => ur.RoleId).Except(model.Roles).ToList();

            if (User.Identity.GetUserId() == user.Id)
            {
                // stop current user from removing their own admin role
                roleIdsToRemove = roleIdsToRemove
                    .Except(RoleManager.Roles.Where(r => r.Name == "tdAdministrators").Select(r => r.Id))
                    .ToList();
            }

            return roleIdsToRemove;
        }

        private static void RemovePendingRoles(UserAccountInfoViewModel model, TicketDeskRole pendingRole)
        {
            // do user info updates
            // if any roles other than pending are selected, make sure we remove the pending role from the model first
            if (pendingRole != null && model.Roles.Any(r => r != pendingRole.Id))
            {
                var roles = model.Roles.ToList();
                roles.Remove(pendingRole.Id);
                model.Roles = roles;
            }
        }

        private async Task<IPagedList<UserAccountInfoViewModel>> GetUsersForListAsync(int page)
        {
            var users = await UserManager.Users
                .OrderBy(u => u.DisplayName)
                .Select(u => new UserAccountInfoViewModel
                {
                    User = u,
                    Roles = u.Roles.Select(r => r.RoleId),
                    IsLocked = (u.LockoutEndDateUtc ?? DateTime.MinValue) > DateTime.UtcNow &&
                               (u.LockoutEndDateUtc ?? DateTime.MinValue) < DateTime.MaxValue,
                    IsDisabled = (u.LockoutEndDateUtc ?? DateTime.MinValue) == DateTime.MaxValue
                })
                .ToPagedListAsync(page, 25).ConfigureAwait(false);
            return users;
        }

        private async Task<bool> UpdateUserInfo(TicketDeskUser user, UserAccountInfoViewModel model)
        {
            user.UserName = model.User.Email;
            user.Email = model.User.Email;
            user.DisplayName = model.User.DisplayName;
            var result = await UserManager.UpdateAsync(user).ConfigureAwait(false);
            if (!result.Succeeded)
            {
                AddErrors(result);
            }

            return result.Succeeded;
        }

        private async Task<bool> AddRoles(IEnumerable<string> roleIdsToAdd, TicketDeskUser user)
        {
            var roleNamesToAdd = RoleManager.Roles.Where(r => roleIdsToAdd.Any(ri => ri == r.Id)).Select(r => r.Name);
            if (roleNamesToAdd.Any())
            {
                var result = await UserManager.AddToRolesAsync(user.Id, await roleNamesToAdd.ToArrayAsync().ConfigureAwait(false)).ConfigureAwait(false);
                if (!result.Succeeded)
                {
                    AddErrors(result);
                }

                return result.Succeeded;
            }

            return true;
        }

        private async Task<bool> RemoveRoles(IEnumerable<string> roleIdsToRemove, TicketDeskUser user)
        {
            var roleNamesToRemove = RoleManager.Roles.Where(r => roleIdsToRemove.Any(ri => ri == r.Id)).Select(r => r.Name);
            if (roleNamesToRemove.Any())
            {
                var result = await UserManager.RemoveFromRolesAsync(user.Id, await roleNamesToRemove.ToArrayAsync().ConfigureAwait(false)).ConfigureAwait(false);
                if (!result.Succeeded)
                {
                    AddErrors(result);
                }

                return result.Succeeded;
            }

            return true;
        }

        private async Task<bool> SetLockout(UserAccountInfoViewModel model, TicketDeskUser user)
        {
            var currentlyLocked = UserManager.IsLockedOut(model.User.Id);
            var currentlyDisabled = currentlyLocked && (user.LockoutEndDateUtc ?? DateTime.MinValue).ToUniversalTime() == DateTime.MaxValue.ToUniversalTime();

            DateTimeOffset newLockoutDate = DateTimeOffset.MinValue;

            if (currentlyDisabled == model.IsDisabled && currentlyLocked == model.IsLocked)
            {
                return true;
            }

            if (model.IsDisabled)
            {
                newLockoutDate = DateTimeOffset.MaxValue;
            }
            else
            {
                if (model.IsLocked)
                {
                    newLockoutDate = DateTimeOffset.Now.Add(UserManager.DefaultAccountLockoutTimeSpan);
                }
            }

            var result = await UserManager.SetLockoutEndDateAsync(model.User.Id, newLockoutDate).ConfigureAwait(false);
            if (!result.Succeeded)
            {
                AddErrors(result);
            }

            return result.Succeeded;
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