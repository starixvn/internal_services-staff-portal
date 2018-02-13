namespace TicketDesk.Web.Client.Controllers
{
    using System.Data.Entity.Migrations;
    using System.Web.Mvc;

    public class NapUserSettingController : BaseController
    {
        public ActionResult SetIsSidebarExpanded(bool isSidebarExpanded)
        {
            NapUserSetting.SettingObject.IsSidebarExpanded = isSidebarExpanded;
            if (NapUserSetting.UserId != 0)
            {
                // Update
                context.NapUserSettings.AddOrUpdate(NapUserSetting);
            }
            else
            {
                // Add
                context.NapUserSettings.Add(NapUserSetting);
            }

            return Json(true);
        }
    }
}