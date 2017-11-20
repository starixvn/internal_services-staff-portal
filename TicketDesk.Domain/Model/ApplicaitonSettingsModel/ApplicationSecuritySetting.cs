namespace TicketDesk.Domain.Model
{
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using TicketDesk.Localization.Domain;

    public class ApplicationSecuritySetting
    {
        private ICollection<string> _defaultNewUserRoles;

        public ApplicationSecuritySetting()
        {
            DefaultNewUserRoles = new List<string>(new[] { "TdPendingUsers" });
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
                var jsonData = JsonConvert.DeserializeObject<ApplicationSecuritySetting>(value, jsettings);
                DefaultNewUserRoles = jsonData.DefaultNewUserRoles;
            }
        }

        [NotMapped]
        [Display(Name = "DefaultNewUserRoles", ResourceType = typeof(Strings))]
        public ICollection<string> DefaultNewUserRoles
        {
            get
            {
                return _defaultNewUserRoles;
            }

            set
            {
                if (!value.Contains("TdPendingUsers"))
                {
                    value.Add("TdPendingUsers");
                }

                _defaultNewUserRoles = value;
            }
        }
    }
}