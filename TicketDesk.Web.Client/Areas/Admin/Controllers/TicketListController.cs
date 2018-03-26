namespace TicketDesk.Web.Client.Areas.Admin.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Web.Mvc;
    using Client.Controllers;
    using Core.Enumerations;
    using Core.Resources;
    using Domain.Infrastructure;
    using Domain.Model;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Web.Attribute;
    using IO;
    using Localization.Client;
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

        [NapAuth(FunctionKey = FunctionKeys.TicketList, Policy = Constant.Policy.Read)]
        public ActionResult Index()
        {
            var viewModel = new TicketListFilterViewModel
            {
                Shops = shopService.GetShopSelectList(),
                Types = GetCategorySelectListByPermission()
            };

            return View(viewModel);
        }

        [NapAuth(FunctionKey = FunctionKeys.TicketList, Policy = Constant.Policy.Read)]
        public ActionResult GetTicketList(TicketListFilterViewModel viewModel)
        {
            var criteria = viewModel.GetQueryCriteria();
            var tickets = ticketService.GetTicketByFilter(criteria);
            foreach (var ticket in tickets)
            {
                ticket.IsHostPermissionOnTicketType = IsHaveHostPermission(ticket.TicketType.ToString(), Constant.Policy.Host);
            }
            return PartialView("_TicketList", tickets);
        }

        [HttpPost]
        [NapAuth(FunctionKey = FunctionKeys.TicketList, Policy = Constant.Policy.Host)]
        public JsonResult UpdateTicketStatus(int id, TicketStatus status)
        {
            if (IsHostPermissionOnTicket(id))
            {
                bool isSuccessful = ticketService.UpdateTicketStatus(id, status);
                string message = isSuccessful ? Messages.UpdateTicketStatusSuccessful : Messages.Error;

                return Json(new { isSuccessful, message }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { message = Resources.NoPermissionForTicketType });
        }

        [NapAuth(FunctionKey = FunctionKeys.TicketList, Policy = Constant.Policy.Read)]
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

        private bool IsHostPermissionOnTicket(int id)
        {
            if (id > 0)
            {
                var ticket = ticketService.GetTicket(id);
                TicketCategoryTypes ticketType = (TicketCategoryTypes)ticket.CategoryId;
                var isHost = IsHaveHostPermission(ticketType.ToString(), Constant.Policy.Host);
                return isHost;
            }
            return false;
        }

        private IEnumerable<SelectListItem> GetCategorySelectListByPermission()
        {
            var selectList = new List<SelectListItem>();
            foreach (TicketCategoryTypes type in Enum.GetValues(typeof(TicketCategoryTypes)))
            {
                if (IsHaveHostPermission(type.ToString(), Constant.Policy.Read))
                {
                    selectList.Add(new SelectListItem
                    {
                        Value = ((int)type).ToString(),
                        Text = type.ToString()
                    });
                }
            }

            var enumLength = Enum.GetValues(typeof(TicketCategoryTypes)).Length;
            if (enumLength == selectList.Count)
            {
                selectList.Add(new SelectListItem
                {
                    Value = null,
                    Text = "All"
                });
            }

            return selectList.OrderBy(x => x.Value);
        }
    }
}