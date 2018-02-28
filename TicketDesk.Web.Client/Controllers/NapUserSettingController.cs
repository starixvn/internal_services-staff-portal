namespace TicketDesk.Web.Client.Controllers
{
    using System.Data.Entity.Migrations;
    using System.Web.Mvc;
    using Domain;

    public class NapUserSettingController : BaseController
    {
        public ActionResult SetIsSidebarExpanded(bool isSidebarExpanded)
        {
            var newNapUserSettingObject = new NapUserSettingObject
            {
                IsSidebarExpanded = isSidebarExpanded,
                ReadAnnouncements = NapUserSetting.SettingObject.ReadAnnouncements
            };

            NapUserSetting.SettingObject = newNapUserSettingObject;
            if (NapUserSetting.UserId != 0)
            {
                // Update
                context.NapUserSettings.AddOrUpdate(NapUserSetting);
            }
            else
            {
                // Add
                NapUserSetting.UserId = CurrentSession.UserId;
                context.NapUserSettings.Add(NapUserSetting);
            }
            context.SaveChanges();

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SaveUnreadAnnouncement(int id)
        {
            string readAnnouncements = NapUserSetting.SettingObject.ReadAnnouncements ?? string.Empty;
            string newReadAnnouncement = !string.IsNullOrEmpty(readAnnouncements) ? string.Concat(",", id.ToString()) : id.ToString();
            string finalReadAnnouncements = string.Concat(readAnnouncements, newReadAnnouncement);
            var newNapUserSettingObject = new NapUserSettingObject
            {
                IsSidebarExpanded = NapUserSetting.SettingObject.IsSidebarExpanded,
                ReadAnnouncements = finalReadAnnouncements
            };

            NapUserSetting.SettingObject = newNapUserSettingObject;

            if (NapUserSetting.UserId != 0)
            {
                // Update
                context.NapUserSettings.AddOrUpdate(NapUserSetting);
            }
            else
            {
                // Add
                NapUserSetting.UserId = CurrentSession.UserId;
                context.NapUserSettings.Add(NapUserSetting);
            }
            context.SaveChanges();

            return Json(true, JsonRequestBehavior.AllowGet);
        }
    }
}