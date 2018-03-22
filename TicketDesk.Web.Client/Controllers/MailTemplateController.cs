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

        public MailTemplateController(ITicketService ticketService)
        {
            this.ticketService = ticketService;
        }

        public ActionResult GeneralTicketDetail(int ticketId)
        {
            var ticket = ticketService.GetTicket(ticketId);

            ////var customFieldValues = context.CustomFieldValues.Where(x => x.TicketId == ticket.TicketId);
            var customFieldValues =
                (from cfv in context.CustomFieldValues
                    join cf in context.CustomFields on cfv.CustomFieldId equals cf.Id
                    where cfv.TicketId == ticketId
                    orderby cf.DisplayOrder, cfv.Id
                    select new
                    {
                        CustomField = cf,
                        cfv.Value,
                        cfv.GroupId
                    }).ToList().Select(x => new CustomFieldValue
                {
                    CustomField = x.CustomField,
                    Value = x.Value,
                    GroupId = x.GroupId
                });

            var singleTicketViewModel = new SingleTicketViewModel
            {
                Ticket = ticket,
                CustomFieldValues = customFieldValues,
                ShopName = context.Shops
                    .Where(x => x.ShopId == ticket.ShopId)
                    .Select(x => x.ShopName)
                    .First()
            };

            return View(singleTicketViewModel);
        }
    }
}