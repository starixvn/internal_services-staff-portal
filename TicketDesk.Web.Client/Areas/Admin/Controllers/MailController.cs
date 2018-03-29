namespace TicketDesk.Web.Client.Areas.Admin.Controllers
{
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
        private readonly IMailService mailService;

        public MailController(ITicketCategoryService ticketCategoryService, IMailService mailService)
        {
            this.ticketCategoryService = ticketCategoryService;
            this.mailService = mailService;
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
            if (!mailService.IsEmailListValid(viewModel.SelectedTicketCategory.MailTo))
            {
                ModelState.AddModelError(
                    $"{ticketCategoryName}.{nameof(viewModel.SelectedTicketCategory.MailTo)}",
                    Resources.ToEmailErrorMessage);
            }

            if (!mailService.IsEmailListValid(viewModel.SelectedTicketCategory.MailCcTo))
            {
                ModelState.AddModelError(
                    $"{ticketCategoryName}.{nameof(viewModel.SelectedTicketCategory.MailCcTo)}",
                    Resources.CcEmailErrorMessage);
            }

            if (!mailService.IsEmailListValid(viewModel.SelectedTicketCategory.MailBccTo))
            {
                ModelState.AddModelError(
                    $"{ticketCategoryName}.{nameof(viewModel.SelectedTicketCategory.MailBccTo)}",
                    Resources.BccEmailErrorMessage);
            }

            if (ModelState.IsValid)
            {
                viewModel.IsSuccessful = ticketCategoryService.UpdateTicketCategory(viewModel.SelectedTicketCategory);
            }

            return PartialView("_ListMail", viewModel);
        }
    }
}