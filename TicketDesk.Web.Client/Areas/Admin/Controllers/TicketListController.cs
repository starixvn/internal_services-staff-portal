namespace TicketDesk.Web.Client.Areas.Admin.Controllers
{
    using System.Web.Mvc;
    using Client.Controllers;
    using Domain.Model;
    using Models.TicketList;
    using Service.Interface;

    public class TicketListController : BaseController
    {
        private readonly IShopService shopService;

        private readonly ITicketService ticketService;

        public TicketListController(IShopService shopService, ITicketService ticketService)
        {
            this.shopService = shopService;
            this.ticketService = ticketService;
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

        public JsonResult UpdateTicketStatus(int id, TicketStatus status)
        {
            bool isSuccessful = ticketService.UpdateTicketStatus(id, status);
            string message = isSuccessful ? "Ticket status has been successfully updated." : "Update failed";

            return Json(new { isSuccessful, message });
        }
    }
}