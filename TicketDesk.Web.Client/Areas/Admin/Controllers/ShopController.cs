namespace TicketDesk.Web.Client.Areas.Admin.Controllers
{
    using System;
    using System.Linq;
    using System.Web.Mvc;
    using Client.Controllers;
    using Core.Enumerations;
    using Core.Resources;
    using Criteria.Shop;
    using Domain.Infrastructure;
    using Domain.Model;
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
                IsHostPermission = IsHavePermission(FunctionKeys.ShopManagement, Constant.Policy.Host)
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
                IsHostPermission = IsHavePermission(FunctionKeys.ShopManagement, Constant.Policy.Host),
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
            var model = new Shop();
            return PartialView("_Create", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateOnlyIncomingValues]
        [NapAuth(FunctionKey = FunctionKeys.ShopManagement, Policy = Constant.Policy.Host)]
        public ActionResult Create(Shop model)
        {
            bool isSuccessful = false;
            string message = string.Empty;
            if (shopService.CheckExistShopName(model.ShopName))
            {
                ModelState.AddModelError("ShopName", Messages.ShopExist);
            }

            if (ModelState.IsValid)
            {
                model.LastUpdatedBy = CurrentSession.Username;
                model.LastUpdatedDate = DateTime.Now;
                isSuccessful = shopService.CreateNewShop(model);
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
                return PartialView("_Edit", shop);
            }

            return HttpNotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateOnlyIncomingValues]
        [NapAuth(FunctionKey = FunctionKeys.ShopManagement, Policy = Constant.Policy.Host)]
        public ActionResult Edit(int id, Shop model)
        {
            bool isSuccessful = false;
            string message = string.Empty;
            var shop = shopService.GetShopById(id);

            if (shop != null && shop.ShopName != model.ShopName && shopService.CheckExistShopName(model.ShopName))
            {
                ModelState.AddModelError("ShopName", Messages.ShopExist);
            }

            if (ModelState.IsValid)
            {
                isSuccessful = shopService.UpdateShop(id, model);
                message = isSuccessful ? Messages.UpdateShopSuccess : Messages.Error;
            }

            var errors = ModelState
                .Where(item => item.Value.Errors.Any())
                .Select(item => new { item.Key, item.Value.Errors.FirstOrDefault()?.ErrorMessage });

            return Json(new { isSuccessful, message, errors });
        }
    }
}