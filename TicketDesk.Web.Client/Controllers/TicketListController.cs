namespace TicketDesk.Web.Client.Controllers
{
    using System.Globalization;
    using System.Web.Mvc;
    using Domain.Model;
    using IO;
    using Models.TicketList;
    using Service.Interface;

    public class TicketListController : BaseController
    {
        private readonly ITicketService ticketService;
        private readonly ICustomFieldService customFieldService;

        public TicketListController(ITicketService ticketService, ICustomFieldService customFieldService)
        {
            this.ticketService = ticketService;
            this.customFieldService = customFieldService;
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

        public ActionResult TicketDetails(int ticketId)
        {
            var ticket = ticketService.GetTicket(ticketId);

            var singleTicketViewModel = new SingleTicketViewModel
            {
                Ticket = ticket,
                CustomFieldValues = customFieldService.GetCustomFieldValuesForTicket(ticket),
                Attachments = TicketDeskFileStore.ListAttachmentInfo(ticketId.ToString(CultureInfo.InvariantCulture), false)
            };

            return View(singleTicketViewModel);
        }
    }
}