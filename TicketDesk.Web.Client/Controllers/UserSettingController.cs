namespace TicketDesk.Web.Client.Controllers
{
    using System.Web.Mvc;
    using Service.Interface;

    public class UserSettingController : BaseController
    {
        private readonly INapUserSettingService napUserSettingService;

        public UserSettingController(INapUserSettingService napUserSettingService, IAnnouncementService announcementService)
        {
            this.napUserSettingService = napUserSettingService;
        }

        public ActionResult SetIsSidebarExpanded(bool isSidebarExpanded)
        {
            var result = napUserSettingService.UpdateIsSideBarExpanded(CurrentSession.UserId, isSidebarExpanded);

            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}