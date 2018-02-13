namespace TicketDesk.Domain
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Newtonsoft.Json.Linq;

    public class NapUserSetting
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int UserId { get; set; }

        public string JsonSetting { get; set; }

        [NotMapped]
        public NapUserSettingObject SettingObject
        {
            get
            {
                var jObject = JObject.Parse(JsonSetting);
                return jObject.ToObject<NapUserSettingObject>();
            }
            set
            {
                var jObject = JObject.FromObject(value);
                JsonSetting = jObject.ToString();
            }
        }
    }

    public class NapUserSettingObject
    {
        public bool IsSidebarExpanded { get; set; }
    }
}
