namespace TicketDesk.Domain.Model
{
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using TicketDesk.Localization.Domain;

    public class ApplicationSelectListSetting
    {
        public ApplicationSelectListSetting()
        {
            // To ensure that missing database values for settings do not completely brick the
            // entire instance, make sure all default settings are initialized in the ctor
            CategoryList = new List<string>(new[] { "Software", "Hardware", "Network" });
            PriorityList = new List<string>(new[] { "High", "Medium", "Low" });
            TicketTypesList = new List<string>(new[] { "Problem", "Question", "Request" });
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
                var jsonData = JsonConvert.DeserializeObject<ApplicationSelectListSetting>(value, jsettings);
                CategoryList = jsonData.CategoryList;
                PriorityList = jsonData.PriorityList;
                TicketTypesList = jsonData.TicketTypesList;
            }
        }

        [NotMapped]
        [Display(Name = "SelectList_Categories", ResourceType = typeof(Strings))]
        public ICollection<string> CategoryList { get; set; }

        [NotMapped]
        [Display(Name = "SelectList_Priorities", ResourceType = typeof(Strings))]
        public ICollection<string> PriorityList { get; set; }

        [NotMapped]
        [Display(Name = "SelectList_TicketTypes", ResourceType = typeof(Strings))]
        public ICollection<string> TicketTypesList { get; set; }
    }
}