namespace TicketDesk.Web.Client.Areas.Admin.Controllers
{
    using System.Web.Mvc;
    using Client.Controllers;
    using Domain.Infrastructure;
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
                TicketCategories = ticketCategoryService.GetTicketCategories()
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
                result = ticketCategoryService.UpdateTicketCategory(viewModel.SelectedTicketCategory);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}