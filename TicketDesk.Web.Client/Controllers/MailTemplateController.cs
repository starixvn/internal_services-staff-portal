namespace TicketDesk.Web.Client.Controllers
{
    using System.Web.Mvc;
    using Domain.Infrastructure;
    using Domain.Model;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Web.Attribute;
    using IO.FileStorage;
    using Service.Interface;

    [NapAuth(FunctionKey = FunctionKeys.Administration, Policy = Constant.Policy.Read)]
    public class MailTemplateController : BaseController
    {
        private readonly ITicketService ticketService;
        private readonly IShopService shopService;
        private readonly ICustomFieldService customFieldService;
        private readonly ITicketEventService ticketEventService;

        public MailTemplateController(
            ITicketService ticketService,
            IShopService shopService,
            ICustomFieldService customFieldService,
            ITicketEventService ticketEventService)
        {
            this.ticketService = ticketService;
            this.shopService = shopService;
            this.customFieldService = customFieldService;
            this.ticketEventService = ticketEventService;
        }

        public ActionResult GeneralTicketDetail(int ticketId)
        {
            var ticket = ticketService.GetTicket(ticketId);

            var createdBy = context.SecurityProvider.NapContextHelper.UserService.GetById(int.Parse(ticket.CreatedBy));
            var lastUpdateBy = context.SecurityProvider.NapContextHelper.UserService.GetById(int.Parse(ticket.LastUpdateBy));

            var singleTicketViewModel = new SingleTicketViewModel
            {
                Ticket = ticket,
                CustomFieldValues = customFieldService.GetCustomFieldValuesForTicket(ticket),
                ShopName = shopService.GetShopById(ticket.ShopId).ShopName,
                CreatedByUsername = createdBy.Username,
                UpdatedByUsername = lastUpdateBy.Username,
                Attachments = TicketDeskFileStore.ListAttachmentInfo(ticketId.ToString(), false),
                TicketEditedMessage = ticketEventService.GetLastStatusChangeTicketEventMessage(ticket.TicketId)
            };

            return View(singleTicketViewModel);
        }
    }
}