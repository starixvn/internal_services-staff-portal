namespace TicketDesk.Web.Client.Areas.Admin.Controllers
{
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using System.Web.Mvc;
    using Client.Controllers;
    using Core.Enumerations;
    using Domain.Infrastructure;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Web.Attribute;
    using Microsoft.Practices.ObjectBuilder2;
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
            var viewModel = new ListMailViewModel
            {
                SelectedTicketCategory = ticketCategoryService.GetTicketCategory(categoryId)
            };

            return PartialView("_ListMail", viewModel);
        }

        [NapAuth(FunctionKey = FunctionKeys.MailManagement, Policy = Constant.Policy.Host)]
        [HttpPost]
        public ActionResult EditMailSettings(ListMailViewModel viewModel)
        {
            var result = false;

            if (ModelState.IsValid)
            {
                viewModel.SelectedTicketCategory.MailTo = RemoveWrongEmailAddresses(viewModel.SelectedTicketCategory.MailTo);
                viewModel.SelectedTicketCategory.MailCcTo = RemoveWrongEmailAddresses(viewModel.SelectedTicketCategory.MailCcTo);
                viewModel.SelectedTicketCategory.MailBccTo = RemoveWrongEmailAddresses(viewModel.SelectedTicketCategory.MailBccTo);

                result = ticketCategoryService.UpdateTicketCategory(viewModel.SelectedTicketCategory);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private string RemoveWrongEmailAddresses(string emails)
        {
            List<string> validMails = new List<string>();
            string[] emailStrings = emails.Split(',', ' ', ';');

            foreach (string emailString in emailStrings)
            {
                if (IsEmailValid(emailString.Trim()))
                {
                    validMails.Add(emailString.Trim());
                }
            }

            return validMails.JoinStrings(";");
        }

        private static bool IsEmailValid(string email)
        {
            return Regex.IsMatch(email, @"/^[^\s@]+@[^\s@]+\.[^\s@]+$/");
        }
    }
}