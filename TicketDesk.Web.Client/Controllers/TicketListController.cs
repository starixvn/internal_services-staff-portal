namespace TicketDesk.Web.Client.Controllers
{
    using System.Web.Mvc;
    using Models.TicketList;
    using Service.Interface;

    public class TicketListController : BaseController
    {
        private readonly ITicketService ticketService;

        public TicketListController(ITicketService ticketService)
        {
            this.ticketService = ticketService;
        }

        public ActionResult Index(TicketListViewModel viewModel)
        {
            var ticketListViewModel = viewModel ?? new TicketListViewModel();

            return View(ticketListViewModel);
        }

        [HttpPost]
        public PartialViewResult TicketList(TicketListViewModel viewModel)
        {
            var tickets = ticketService.GetTicketByStatus(CurrentSession.UserId, viewModel.Status, viewModel.TicketType);
            return PartialView("_TicketList", tickets);
        }

        public ActionResult TicketDetails(int id)
        {
            var singleTicketViewModel = ticketService.GetTicketDetail(id);

            return View(singleTicketViewModel);
        }
    }
}