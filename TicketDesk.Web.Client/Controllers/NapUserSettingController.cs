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
                IsSidebarExpanded = isSidebarExpanded
                // Copy other properties here
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