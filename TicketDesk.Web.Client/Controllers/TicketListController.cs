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
        private readonly IShopService shopService;

        public TicketListController(ITicketService ticketService, ICustomFieldService customFieldService, IShopService shopService)
        {
            this.ticketService = ticketService;
            this.customFieldService = customFieldService;
            this.shopService = shopService;
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

        public ActionResult TicketDetails(int ticketId)
        {
            var ticket = ticketService.GetTicket(ticketId);
            var shopName = string.Empty;
            if (ticket != null)
            {
                var shop = shopService.GetShopById(ticket.ShopId);
                shopName = shop.ShopName;
            }

            var singleTicketViewModel = new SingleTicketViewModel
            {
                Ticket = ticket,
                ShopName = shopName,
                CustomFieldValues = customFieldService.GetCustomFieldValuesForTicket(ticket),
                Attachments = TicketDeskFileStore.ListAttachmentInfo(ticketId.ToString(CultureInfo.InvariantCulture), false)
            };

            return View(singleTicketViewModel);
        }
    }
}