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
            ticketListViewModel.Tickets = ticketService.GetTicketByStatus(CurrentSession.UserId,
                                                                          ticketListViewModel.Skipped,
                                                                          ticketListViewModel.ItemPerPage,
                                                                          ticketListViewModel.Status);
            ticketListViewModel.TotalItem = ticketService.GetUserTotalTicketByStatus(CurrentSession.UserId, ticketListViewModel.Status);

            return View(ticketListViewModel);
        }
    }
}