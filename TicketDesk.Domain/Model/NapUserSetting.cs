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

        public string JsonSetting
        {
            get
            {
                var jObject = JObject.FromObject(SettingObject);
                return jObject.ToString();
            }
            set
            {
                var jObject = JObject.Parse(value);
                SettingObject = jObject.ToObject<NapUserSettingObject>();
            }
        }

        [NotMapped]
        public NapUserSettingObject SettingObject { get; set; }
    }

    public class NapUserSettingObject
    {
        public bool IsSidebarExpanded { get; set; }
    }
}