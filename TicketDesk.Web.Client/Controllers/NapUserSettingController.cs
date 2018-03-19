namespace TicketDesk.Web.Client.Controllers
{
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

        public ActionResult SaveUnreadAnnouncement(int id)
        {
            var result = napUserSettingService.UpdateUnreadAnnouncements(CurrentSession.UserId, id);
            var unreadQuantity = announcementService.CountUnreadAnnouncement(NapUserSetting.SettingObject.ReadAnnouncements);

            return Json(new { result, unreadQuantity }, JsonRequestBehavior.AllowGet);
        }
    }
}