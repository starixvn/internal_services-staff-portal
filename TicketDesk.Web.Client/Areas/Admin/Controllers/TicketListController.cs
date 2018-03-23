namespace TicketDesk.Web.Client.Areas.Admin.Controllers
{
    using System.Globalization;
    using System.Web.Mvc;
    using Client.Controllers;
    using Core.Resources;
    using Domain.Model;
    using IO;
    using Models.TicketList;
    using Service.Interface;

    public class TicketListController : BaseController
    {
        private readonly IShopService shopService;
        private readonly ITicketService ticketService;
        private readonly ICustomFieldService customFieldService;

        public TicketListController(IShopService shopService, ITicketService ticketService, ICustomFieldService customFieldService)
        {
            this.shopService = shopService;
            this.ticketService = ticketService;
            this.customFieldService = customFieldService;
        }

        public ActionResult Index()
        {
            var viewModel = new TicketListFilterViewModel
            {
                Shops = shopService.GetShopSelectList()
            };

            return View(viewModel);
        }

        public ActionResult GetTicketList(TicketListFilterViewModel viewModel)
        {
            var criteria = viewModel.GetQueryCriteria();
            var tickets = ticketService.GetTicketByFilter(criteria);
            return PartialView("_TicketList", tickets);
        }

        [HttpPost]
        public JsonResult UpdateTicketStatus(int id, TicketStatus status)
        {
            bool isSuccessful = ticketService.UpdateTicketStatus(id, status);
            string message = isSuccessful ? Messages.UpdateTicketStatusSuccessful : Messages.Error;

            return Json(new { isSuccessful, message }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult TicketDetails(int ticketId)
        {
            var ticket = ticketService.GetTicket(ticketId);

            var singleTicketViewModel = new SingleTicketViewModel
            {
                Ticket = ticket,
                CustomFieldValues = customFieldService.GetCustomFieldValuesForTicket(ticket),
                Attachments = TicketDeskFileStore.ListAttachmentInfo(ticketId.ToString(CultureInfo.InvariantCulture), false)
            };

            return PartialView("_TicketDetails", singleTicketViewModel);
        }
    }
}