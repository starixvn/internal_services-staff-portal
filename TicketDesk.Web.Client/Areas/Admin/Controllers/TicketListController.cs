namespace TicketDesk.Web.Client.Areas.Admin.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Client.Controllers;
    using Core.Enumerations;
    using Core.Resources;
    using Domain.Extensions;
    using Domain.Infrastructure;
    using Domain.Model;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Web.Attribute;
    using Localization.Client;
    using Models.TicketList;
    using Service.Interface;

    public class TicketListController : BaseController
    {
        private readonly IShopService shopService;
        private readonly ITicketService ticketService;
        private readonly IMailService mailService;

        public TicketListController(IShopService shopService,
            ITicketService ticketService,
            IMailService mailService)
        {
            this.shopService = shopService;
            this.ticketService = ticketService;
            this.mailService = mailService;
        }

        [NapAuth(FunctionKey = FunctionKeys.TicketListAdmin, Policy = Constant.Policy.Read)]
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
                ticket.IsHostPermissionOnTicketType = IsHavePermission(GetFunctionKeyByTicketType(ticket.TicketType), Constant.Policy.Host);
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

                if (isSuccessful)
                {
                    mailService.ComposeAndSendMail(ticketService.GetTicket(id));
                }

                return Json(new { isSuccessful, message }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { isSuccessful = false, message = Resources.NoPermissionForTicketType }, JsonRequestBehavior.AllowGet);
        }

        [NapAuth(FunctionKey = FunctionKeys.TicketList, Policy = Constant.Policy.Read)]
        public ActionResult TicketDetails(int id)
        {
            var singleTicketViewModel = ticketService.GetTicketDetail(id);

            return PartialView("_TicketDetails", singleTicketViewModel);
        }

        private bool IsHostPermissionOnTicket(int id)
        {
            if (id > 0)
            {
                var ticket = ticketService.GetTicket(id);
                TicketCategoryTypes ticketType = (TicketCategoryTypes)ticket.CategoryId;
                var isHost = IsHavePermission(GetFunctionKeyByTicketType(ticketType), Constant.Policy.Host);
                return isHost;
            }

            return false;
        }

        private IEnumerable<SelectListItem> GetCategorySelectListByPermission()
        {
            var selectList = new List<SelectListItem>();
            foreach (TicketCategoryTypes type in Enum.GetValues(typeof(TicketCategoryTypes)))
            {
                if (IsHavePermission(GetFunctionKeyByTicketType(type), Constant.Policy.Read))
                {
                    selectList.Add(new SelectListItem
                    {
                        Value = ((int)type).ToString(),
                        Text = EnumExtensions.GetDisplayName(type)
                    });
                }
            }

            var enumLength = Enum.GetValues(typeof(TicketCategoryTypes)).Length;
            if (enumLength == selectList.Count)
            {
                selectList.Add(new SelectListItem
                {
                    Value = null,
                    Text = Resources.All
                });
            }

            return selectList.OrderBy(x => x.Value);
        }

        private static string GetFunctionKeyByTicketType(TicketCategoryTypes type)
        {
            return string.Format("{0}{1}", type.ToString(), "Admin");
        }
    }
}