namespace TicketDesk.Web.Client.Controllers
{
    using System.Web.Mvc;
    using Service.Interface;

    public class NapUserSettingController : BaseController
    {
        private readonly INapUserSettingService napUserSettingService;

        public NapUserSettingController(INapUserSettingService napUserSettingService)
        {
            this.napUserSettingService = napUserSettingService;
        }

        public ActionResult SetIsSidebarExpanded(bool isSidebarExpanded)
        {
            var result = napUserSettingService.UpdateIsSideBarExpanded(CurrentSession.UserId, isSidebarExpanded);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SaveUnreadAnnouncement(int id)
        {
            var result = napUserSettingService.UpdateUnreadAnnouncements(CurrentSession.UserId, id);

            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}