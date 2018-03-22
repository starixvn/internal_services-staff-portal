namespace TicketDesk.Web.Client.Controllers
{
    using System.Linq;
    using System.Web.Mvc;
    using Domain.Infrastructure;
    using Domain.Model;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Web.Attribute;
    using Service.Interface;

    [NapAuth(FunctionKey = FunctionKeys.Administration, Policy = Constant.Policy.Read)]
    public class MailTemplateController : BaseController
    {
        private readonly ITicketService ticketService;
        private readonly ICustomFieldService customFieldService;

        public MailTemplateController(ITicketService ticketService, ICustomFieldService customFieldService)
        {
            this.ticketService = ticketService;
            this.customFieldService = customFieldService;
        }

        public ActionResult GeneralTicketDetail(int ticketId)
        {
            var ticket = ticketService.GetTicket(ticketId);

            var customFieldValues = context.CustomFieldValues.Where(x => x.TicketId == ticket.TicketId);

            var singleTicketViewModel = new SingleTicketViewModel
            {
                Ticket = ticket,
                CustomFieldValues = customFieldValues
            };

            return View(singleTicketViewModel);
        }
    }
}