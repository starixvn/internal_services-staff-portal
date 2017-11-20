namespace TicketDesk.Domain.Model
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using TicketDesk.Localization;

    public class UserSetting
    {
        [Key]
        [StringLength(256, ErrorMessageResourceName = "FieldMaximumLength", ErrorMessageResourceType = typeof(Validation))]
        public string UserId { get; set; }

        public virtual UserTicketListSettingsCollection ListSettings { get; internal set; }

        public int? SelectedProjectId { get; set; }

        public static UserSetting GetDefaultSettingsForUser(string userId, bool isHelpDeskUser)
        {
            var collection = new UserTicketListSettingsCollection
            {
                UserTicketListSetting.GetDefaultListSettings(userId, isHelpDeskUser)
            };

            return new UserSetting { UserId = userId, ListSettings = collection };
        }

        public UserTicketListSetting GetUserListSettingByName(string listName)
        {
            return ListSettings.FirstOrDefault(us => us.ListName.Equals(listName, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}