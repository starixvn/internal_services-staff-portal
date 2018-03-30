namespace TicketDesk.Web.Client.Controllers
{
    using System.Linq;
    using System.Web.Mvc;
    using Service.Interface;

    public class NapUserSettingController : BaseController
    {
        private readonly INapUserSettingService napUserSettingService;

        private readonly IAnnouncementService announcementService;

        public NapUserSettingController(INapUserSettingService napUserSettingService, IAnnouncementService announcementService)
        {
            this.napUserSettingService = napUserSettingService;
            this.announcementService = announcementService;
        }

        public ActionResult SetIsSidebarExpanded(bool isSidebarExpanded)
        {
            var result = napUserSettingService.UpdateIsSideBarExpanded(CurrentSession.UserId, isSidebarExpanded);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SaveAndCountTotalUnreadAnnouncement(int id)
        {
            var result = announcementService.SetAnnouncementReadStatus(id, CurrentSession.UserId);
            var unreadQuantity = announcementService.GetHomeAnnouncements().Count();

            return Json(new { result, unreadQuantity }, JsonRequestBehavior.AllowGet);
        }
    }
}