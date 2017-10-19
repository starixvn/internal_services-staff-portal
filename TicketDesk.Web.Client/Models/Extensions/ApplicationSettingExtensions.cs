namespace TicketDesk.Domain.Model
{
    using System.Linq;
    using System.Web.Mvc;
    using Web.Client;
    using Web.Identity;

    public static class ApplicationSettingExtensions
    {
        public static SelectList GetPriorityList(this ApplicationSetting settings, bool includeEmpty, string selectedPriority)
        {
            return settings.SelectLists.PriorityList.ToSelectList(p => p, p => p, selectedPriority, includeEmpty);
        }

        public static SelectList GetCategoryList(this ApplicationSetting settings, bool includeEmpty, string selectedCategory)
        {
            return settings.SelectLists.CategoryList.ToSelectList(p => p, p => p, selectedCategory, includeEmpty);
        }

        public static SelectList GetTicketTypeList(this ApplicationSetting settings, bool includeEmpty, string selectedType)
        {
            return settings.SelectLists.TicketTypesList.ToSelectList(p => p, p => p, selectedType, includeEmpty);
        }

        public static MultiSelectList GetDefaultNewUserRolesList(this ApplicationSetting settings)
        {
            var roleManager = DependencyResolver.Current.GetService<TicketDeskRoleManager>();
            return roleManager.Roles.ToMultiSelectList(
                r => r.Name,
                r => r.DisplayName,
                settings.SecuritySettings.DefaultNewUserRoles.ToArray());
        }

        public static string GetPriorities(this ApplicationSetting settings)
        {
            return string.Join(",", settings.SelectLists.PriorityList);
        }

        public static string GetTicketTypes(this ApplicationSetting settings)
        {
            return string.Join(",", settings.SelectLists.TicketTypesList);
        }

        public static string GetCategories(this ApplicationSetting settings)
        {
            return string.Join(",", settings.SelectLists.CategoryList);
        }
    }
}