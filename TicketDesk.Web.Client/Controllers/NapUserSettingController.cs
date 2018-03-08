namespace TicketDesk.Web.Client.Controllers
{
    using System.Collections.Generic;
    using System.Data.Entity.Migrations;
    using System.Linq;
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
            var readAnnouncementIds = new List<int>();

            if (NapUserSetting.SettingObject.ReadAnnouncements != null)
            {
                readAnnouncementIds = NapUserSetting.SettingObject.ReadAnnouncements
                    .Split(',')
                    .Distinct()
                    .Select(int.Parse)
                    .ToList();

                if (!readAnnouncementIds.Contains(id))
                {
                    readAnnouncementIds.Add(id);
                    readAnnouncementIds.Sort();
                }
            }

            var newNapUserSettingObject = new NapUserSettingObject
            {
                IsSidebarExpanded = NapUserSetting.SettingObject.IsSidebarExpanded,
                ReadAnnouncements = string.Join(",", readAnnouncementIds)
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