namespace TicketDesk.Web.Client.Controllers
{
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
        private readonly IShopService shopService;
        private readonly ICustomFieldService customFieldService;

        public MailTemplateController(ITicketService ticketService, IShopService shopService, ICustomFieldService customFieldService)
        {
            this.ticketService = ticketService;
            this.shopService = shopService;
            this.customFieldService = customFieldService;
        }

        public ActionResult GeneralTicketDetail(int ticketId)
        {
            var ticket = ticketService.GetTicket(ticketId);

            //// var customFieldValues =
            ////     (from cfv in context.CustomFieldValues
            ////      join cf in context.CustomFields on cfv.CustomFieldId equals cf.Id
            ////      where cfv.TicketId == ticketId
            ////      orderby cf.DisplayOrder, cfv.Id
            ////      select new
            ////      {
            ////          CustomField = cf,
            ////          cfv.Value,
            ////          cfv.GroupId
            ////      }).ToList().Select(x => new CustomFieldValue
            ////      {
            ////          CustomField = x.CustomField,
            ////          Value = x.Value,
            ////          GroupId = x.GroupId
            ////      });

            var singleTicketViewModel = new SingleTicketViewModel
            {
                Ticket = ticket,
                CustomFieldValues = customFieldService.GetCustomFieldValuesForTicket(ticket),
                ShopName = shopService.GetShopById(ticket.ShopId).ShopName
            };

            return View(singleTicketViewModel);
        }
    }
}