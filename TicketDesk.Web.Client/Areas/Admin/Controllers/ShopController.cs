namespace TicketDesk.Web.Client.Areas.Admin.Controllers
{
    using System.Linq;
    using System.Web.Mvc;
    using Client.Controllers;
    using Core.Enumerations;
    using Core.Resources;
    using Criteria.Shop;
    using Domain.Infrastructure;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Web.Attribute;
    using Models.ShopManagement;
    using Service.Interface;

    [NapAuth(FunctionKey = FunctionKeys.ShopManagement, Policy = Constant.Policy.Read)]
    public class ShopController : BaseController
    {
        private readonly IShopService shopService;

        public ShopController(IShopService shopService)
        {
            this.shopService = shopService;
        }

        public ActionResult Index()
        {
            var viewModel = new ShopFilterViewModel
            {
                ActiveState = ActiveState.All,
                IsHostPermission = IsHaveHostPermission(FunctionKeys.ShopManagement, Constant.Policy.Host)
            };
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult GetListShop(ShopFilterViewModel model)
        {
            var criteria = new ShopFilterCrietria
            {
                ShopId = model.ShopId,
                ShopName = model.ShopName,
                ActiveState = model.ActiveState
            };

            var viewModel = new ShopListViewModel
            {
                IsHostPermission = IsHaveHostPermission(FunctionKeys.ShopManagement, Constant.Policy.Host),
                Shops = shopService.GetListShop(criteria).ToList()
            };
            return PartialView("_ListShop", viewModel);
        }

        public ActionResult GetShopSuggestion(string shopName)
        {
            var shops = shopService.GetShopSuggestion(shopName);
            return Json(shops, JsonRequestBehavior.AllowGet);
        }

        [NapAuth(FunctionKey = FunctionKeys.ShopManagement, Policy = Constant.Policy.Host)]
        public ActionResult Create()
        {
            var viewModel = new ShopViewModel();
            return PartialView("_Create", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [NapAuth(FunctionKey = FunctionKeys.ShopManagement, Policy = Constant.Policy.Host)]
        public ActionResult Create(ShopViewModel viewModel)
        {
            bool isSuccessful = false;
            string message = string.Empty;
            if (shopService.CheckExistShopName(viewModel.ShopName))
            {
                ModelState.AddModelError("ShopName", Messages.ShopExist);
            }

            if (ModelState.IsValid)
            {
                isSuccessful = shopService.CreateNewShop(viewModel.GetCriteria());
                message = isSuccessful ? Messages.CreateShopSuccess : Messages.Error;
            }

            var errors = ModelState
                .Where(item => item.Value.Errors.Any())
                .Select(item => new { item.Key, item.Value.Errors.FirstOrDefault()?.ErrorMessage });

            return Json(new { isSuccessful, message, errors });
        }

        [NapAuth(FunctionKey = FunctionKeys.ShopManagement, Policy = Constant.Policy.Host)]
        public ActionResult Edit(int id)
        {
            var shop = shopService.GetShopById(id);
            if (shop != null)
            {
                var viewModel = new ShopViewModel
                {
                    ShopId = shop.ShopId,
                    ShopName = shop.ShopName,
                    ShopCode = shop.ShopCode,
                    ContactName = shop.ContactName,
                    ContactNumber = shop.ContactNumber,
                    Email = shop.Email,
                    Address = shop.Address,
                    IsActive = shop.IsActive
                };
                return PartialView("_Edit", viewModel);
            }

            return HttpNotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [NapAuth(FunctionKey = FunctionKeys.ShopManagement, Policy = Constant.Policy.Host)]
        public ActionResult Edit(int id, ShopViewModel viewModel)
        {
            bool isSuccessful = false;
            string message = string.Empty;
            var shop = shopService.GetShopById(id);

            if (shop != null && shop.ShopName != viewModel.ShopName && shopService.CheckExistShopName(viewModel.ShopName))
            {
                ModelState.AddModelError("ShopName", Messages.ShopExist);
            }

            if (ModelState.IsValid)
            {
                isSuccessful = shopService.UpdateShop(id, viewModel.GetCriteria());
                message = isSuccessful ? Messages.UpdateShopSuccess : Messages.Error;
            }

            var errors = ModelState
                .Where(item => item.Value.Errors.Any())
                .Select(item => new { item.Key, item.Value.Errors.FirstOrDefault()?.ErrorMessage });

            return Json(new { isSuccessful, message, errors });
        }
    }
}