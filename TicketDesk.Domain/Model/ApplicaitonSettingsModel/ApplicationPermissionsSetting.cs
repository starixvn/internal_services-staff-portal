namespace TicketDesk.Domain.Model
{
    using Newtonsoft.Json;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using TicketDesk.Localization.Domain;

    public class ApplicationPermissionsSetting
    {
        public ApplicationPermissionsSetting()
        {
            // To ensure that missing database values for settings do not completely brick the
            // entire instance, make sure all default settings are initialized in the ctor
            AllowInternalUsersToEditPriority = false;
            AllowInternalUsersToEditTags = true;
            AllowInternalUsersToSetAssigned = false;
            AllowInternalUsersToSetOwner = false;
        }

        [JsonIgnore]
        [Display(AutoGenerateField = false)]
        [ScaffoldColumn(false)]
        public string Serialized
        {
            get
            {
                return JsonConvert.SerializeObject(this);
            }

            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    return;
                }

                var jsettings = new JsonSerializerSettings { ObjectCreationHandling = ObjectCreationHandling.Replace };
                var jsonData = JsonConvert.DeserializeObject<ApplicationPermissionsSetting>(value, jsettings);
                AllowInternalUsersToEditPriority = jsonData.AllowInternalUsersToEditPriority;
                AllowInternalUsersToEditTags = jsonData.AllowInternalUsersToEditTags;
                AllowInternalUsersToSetAssigned = jsonData.AllowInternalUsersToSetAssigned;
                AllowInternalUsersToSetOwner = jsonData.AllowInternalUsersToSetOwner;
            }
        }

        [NotMapped]
        [Display(Name = "AllowInternalUsersToEditPriority", ResourceType = typeof(Strings))]
        public bool AllowInternalUsersToEditPriority { get; set; }

        [NotMapped]
        [Display(Name = "AllowInternalUsersToEditTags", ResourceType = typeof(Strings))]
        public bool AllowInternalUsersToEditTags { get; set; }

        [NotMapped]
        [Display(Name = "AllowInternalUsersToSetAssigned", ResourceType = typeof(Strings))]
        public bool AllowInternalUsersToSetAssigned { get; set; }

        [NotMapped]
        [Display(Name = "AllowInternalUsersToSetOwner", ResourceType = typeof(Strings))]
        public bool AllowInternalUsersToSetOwner { get; set; }
    }
}