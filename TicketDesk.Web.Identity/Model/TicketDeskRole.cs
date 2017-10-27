namespace TicketDesk.Web.Identity.Model
{
    using Localization;
    using Localization.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System.ComponentModel.DataAnnotations;

    public class TicketDeskRole : IdentityRole
    {
        private string _displayName;
        private string _description;

        public TicketDeskRole()
        {
        }

        public TicketDeskRole(string roleName, string displayName, string description)
            : base(roleName)
        {
            _displayName = displayName;
            _description = description;
        }

        [Required(ErrorMessageResourceName = "FieldRequired", ErrorMessageResourceType = typeof(Validation))]
        [StringLength(100, ErrorMessageResourceName = "FieldMaximumLength", ErrorMessageResourceType = typeof(Validation))]
        [Display(Name = "DisplayName", ResourceType = typeof(Strings))]
        public string DisplayName
        {
            get
            {
                return TicketDeskRoleManager.DefaultRolesDisplayName.ContainsKey(Name)
                    ? TicketDeskRoleManager.DefaultRolesDisplayName[Name]
                    : _displayName;
            }

            set
            {
                _displayName = value;
            }
        }

        [StringLength(500, ErrorMessageResourceName = "FieldMaximumLength", ErrorMessageResourceType = typeof(Validation))]
        [Display(Name = "RoleDescription", ResourceType = typeof(Strings))]
        public string Description
        {
            get
            {
                return TicketDeskRoleManager.DefaultRolesDescription.ContainsKey(Name)
                    ? TicketDeskRoleManager.DefaultRolesDescription[Name]
                    : _description;

            }
            set
            {
                _description = value;
            }
        }
    }
}