namespace TicketDesk.Domain.Model
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class ApplicationSetting
    {
        public ApplicationSetting()
        {
            // To ensure that missing database values for settings do not completely brick the
            // entire instance, make sure all default settings are initialized in the ctor
            ApplicationName = "TicketDesk";
            Permissions = new ApplicationPermissionsSetting();
            SelectLists = new ApplicationSelectListSetting();
            SecuritySettings = new ApplicationSecuritySetting();
            ClientSettings = new ClientSetting();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Display(AutoGenerateField = false)]
        public string ApplicationName { get; set; }

        public ApplicationPermissionsSetting Permissions { get; set; }

        public ApplicationSelectListSetting SelectLists { get; set; }

        public ApplicationSecuritySetting SecuritySettings { get; set; }

        public ClientSetting ClientSettings { get; set; }
    }
}