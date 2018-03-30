namespace TicketDesk.Web.Client.Controllers
{
    using System.Web.Mvc;
    using Service.Interface;

    public class UserSettingController : BaseController
    {
        private readonly IUserSettingService userSettingService;

        public UserSettingController(IUserSettingService userSettingService, IAnnouncementService announcementService)
        {
            this.userSettingService = userSettingService;
        }

        public ActionResult SetIsSidebarExpanded(bool isSidebarExpanded)
        {
            var result = userSettingService.UpdateIsSideBarExpanded(CurrentSession.UserId, isSidebarExpanded);

            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}