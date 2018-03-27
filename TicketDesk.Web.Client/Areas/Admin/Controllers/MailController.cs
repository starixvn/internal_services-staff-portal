namespace TicketDesk.Web.Client.Areas.Admin.Controllers
{
    using System.Linq;
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

        [HttpPost]
        [NapAuth(FunctionKey = FunctionKeys.MailManagement, Policy = Constant.Policy.Host)]
        public ActionResult EditMailSettings(TicketCategory ticketCategory)
        {
            var result = false;

            if (IsEmailListValid(ticketCategory.MailTo))
            {
                ModelState.AddModelError(nameof(ticketCategory.MailTo), "Email not valid");
            }

            if (IsEmailListValid(ticketCategory.MailCcTo))
            {
                ModelState.AddModelError(nameof(ticketCategory.MailCcTo), "Email not valid");
            }

            if (IsEmailListValid(ticketCategory.MailBccTo))
            {
                ModelState.AddModelError(nameof(ticketCategory.MailBccTo), "Email not valid");
            }

            if (ModelState.IsValid)
            {
                result = ticketCategoryService.UpdateTicketCategory(ticketCategory);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private static bool IsEmailListValid(string emails)
        {
            var result = true;

            if (!string.IsNullOrEmpty(emails))
            {
                var emailStrings = emails.Split(',', ' ', ';');

                if (emailStrings.Any(emailString => !IsEmailValid(emailString.Trim())))
                {
                    result = false;
                }
            }

            return result;
        }

        private static bool IsEmailValid(string email)
        {
            return Regex.IsMatch(email, @"^[^\s@]+@[^\s@]+\.[^\s@]+$");
        }
    }
}