namespace TicketDesk.Web.Client.Areas.Admin.Controllers
{
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using System.Web.Mvc;
    using Client.Controllers;
    using Core.Enumerations;
    using Domain.Infrastructure;
    using Domain.Model;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Web.Attribute;
    using Models.Mail;
    using Service.Interface;

    public class MailController : BaseController
    {
        private readonly ITicketCategoryService ticketCategoryService;

        public MailController(ITicketCategoryService ticketCategoryService)
        {
            this.ticketCategoryService = ticketCategoryService;
        }

        [NapAuth(FunctionKey = FunctionKeys.MailManagement, Policy = Constant.Policy.Read)]
        public ActionResult Index()
        {
            var viewModel = new MailViewModel
            {
                CategoryId = TicketCategoryTypes.Facility
            };

            return View(viewModel);
        }

        [NapAuth(FunctionKey = FunctionKeys.MailManagement, Policy = Constant.Policy.Read)]
        public ActionResult ListMail(int categoryId)
        {
            var ticketCategory = ticketCategoryService.GetTicketCategory(categoryId);
            return PartialView("_ListMail", ticketCategory);
        }

        [NapAuth(FunctionKey = FunctionKeys.MailManagement, Policy = Constant.Policy.Host)]
        [HttpPost]
        public ActionResult EditMailSettings(TicketCategory ticketCategory)
        {
            var result = false;

            if (ModelState.IsValid)
            {
                ticketCategory.MailTo = RemoveWrongEmailAddresses(ticketCategory.MailTo);
                ticketCategory.MailCcTo = RemoveWrongEmailAddresses(ticketCategory.MailCcTo);
                ticketCategory.MailBccTo = RemoveWrongEmailAddresses(ticketCategory.MailBccTo);

                result = ticketCategoryService.UpdateTicketCategory(ticketCategory);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private static string RemoveWrongEmailAddresses(string emails)
        {
            if (string.IsNullOrEmpty(emails))
            {
                return null;
            }

            List<string> validMails = new List<string>();
            var emailStrings = emails.Split(',', ' ', ';');

            foreach (var emailString in emailStrings)
            {
                if (IsEmailValid(emailString.Trim()))
                {
                    validMails.Add(emailString.Trim());
                }
            }

            return string.Join(";", validMails);
        }

        private static bool IsEmailValid(string email)
        {
            return Regex.IsMatch(email, @"^[^\s@]+@[^\s@]+\.[^\s@]+$");
        }
    }
}