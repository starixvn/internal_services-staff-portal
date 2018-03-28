namespace TicketDesk.Web.Client.Areas.Admin.Controllers
{
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Web.Mvc;
    using Client.Controllers;
    using Core.Enumerations;
    using Domain.Infrastructure;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Web.Attribute;
    using Localization.Client;
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
            var viewModel = new MailListViewModel
            {
                SelectedTicketCategory = ticketCategoryService.GetTicketCategory(categoryId),
                IsHaveHostPermission = IsHavePermission(FunctionKeys.MailManagement, Constant.Policy.Host)
            };

            return PartialView("_ListMail", viewModel);
        }

        [HttpPost]
        [NapAuth(FunctionKey = FunctionKeys.MailManagement, Policy = Constant.Policy.Host)]
        public ActionResult EditMailSettings(MailListViewModel viewModel)
        {
            viewModel.IsHaveHostPermission = IsHavePermission(FunctionKeys.MailManagement, Constant.Policy.Host);

            var ticketCategoryName = nameof(viewModel.SelectedTicketCategory);
            if (!IsEmailListValid(viewModel.SelectedTicketCategory.MailTo))
            {
                ModelState.AddModelError(
                    $"{ticketCategoryName}.{nameof(viewModel.SelectedTicketCategory.MailTo)}",
                    Resources.ToEmailErrorMessage);
            }

            if (!IsEmailListValid(viewModel.SelectedTicketCategory.MailCcTo))
            {
                ModelState.AddModelError(
                    $"{ticketCategoryName}.{nameof(viewModel.SelectedTicketCategory.MailCcTo)}",
                    Resources.CcEmailErrorMessage);
            }

            if (!IsEmailListValid(viewModel.SelectedTicketCategory.MailBccTo))
            {
                ModelState.AddModelError(
                    $"{ticketCategoryName}.{nameof(viewModel.SelectedTicketCategory.MailBccTo)}",
                    Resources.BccEmailErrorMessage);
            }

            if (ModelState.IsValid)
            {
                viewModel.IsSuccess = ticketCategoryService.UpdateTicketCategory(viewModel.SelectedTicketCategory);
            }

            return PartialView("_ListMail", viewModel);
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