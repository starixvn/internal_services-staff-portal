namespace TicketDesk.Web.Client.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Web.Mvc;
    using Localization;
    using Localization.Models;
    using Identity;
    using Identity.Model;

    public class UserAccountInfoViewModel
    {
        public UserAccountInfoViewModel()
        {
            Roles = new string[] { };
        }

        public UserAccountInfoViewModel(TicketDeskUser user, IEnumerable<string> roles)
        {
            var lockDate = (user.LockoutEndDateUtc ?? DateTime.MinValue).ToUniversalTime();
            User = user;
            IsLocked = lockDate > DateTime.UtcNow && lockDate < DateTime.MaxValue.ToUniversalTime();
            IsDisabled = lockDate == DateTime.MaxValue.ToUniversalTime();
            Roles = roles ?? new string[] { };
        }

        public TicketDeskUser User { get; set; }

        [Display(Name = "Locked", Prompt = "Locked_Prompt", ResourceType = typeof(Strings))]
        [LocalizedDescription("IsLocked_Description", NameResourceType = typeof(Strings))]
        public bool IsLocked { get; set; }

        [Display(Name = "Disabled", Prompt = "Disabled_Prompt", ResourceType = typeof(Strings))]
        [LocalizedDescription("IsDisabled_Description", NameResourceType = typeof(Strings))]
        public bool IsDisabled { get; set; }

        public IEnumerable<string> Roles { get; set; }

        public MultiSelectList UserRolesList
        {
            get
            {
                var roleManager = DependencyResolver.Current.GetService<TicketDeskRoleManager>();

                return roleManager.Roles.ToMultiSelectList(
                    r => r.Id,
                    r => r.DisplayName,
                    Roles.ToArray());
            }
        }

        public IEnumerable<string> GetRoleNames(IEnumerable<TicketDeskRole> allRolesList)
        {
            return allRolesList.Where(ar => Roles.Any(r => r == ar.Id)).Select(ar => ar.DisplayName);
        }
    }
}