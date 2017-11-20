namespace TicketDesk.Domain.Model
{
    using System.Data.Entity;
    using System.Linq;

    public static class ApplicationSettingExtensions
    {
        public static ApplicationSetting GetTicketDeskSettings(this DbSet<ApplicationSetting> settings)
        {
            return settings.FirstOrDefault(s => s.ApplicationName == "TicketDesk");
        }
    }
}