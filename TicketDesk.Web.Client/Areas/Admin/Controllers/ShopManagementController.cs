namespace TicketDesk.Web.Client.Areas.Admin.Controllers
{
    using System.Web.Mvc;
    using Client.Controllers;
    using Core.Enumerations;
    using Domain.Infrastructure;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Web.Attribute;
    using Models.ShopManagement;

    [NapAuth(FunctionKey = FunctionKeys.ShopManagement, Policy = Constant.Policy.Read)]
    public class ShopManagementController : BaseController
    {
        // GET: Admin/Shop
        public ActionResult Index()
        {
            var viewModel = new ShopManagementViewModel
            {
                IsActive = ActiveState.All
            };
            return View(viewModel);
        }

        public ActionResult GetListShop(ShopManagementViewModel viewModel)
        {
            return PartialView("_ListShop");
        }
    }
}