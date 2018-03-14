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
        private readonly IShopService _shopService;

        public ShopController(IShopService shopService)
        {
            _shopService = shopService;
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

            var shops = _shopService.GetListShop(criteria);

            var viewModel = new ShopListViewModel
            {
                IsHostPermission = IsHaveHostPermission(FunctionKeys.ShopManagement, Constant.Policy.Host),
                Shops = shops.OrderBy(s => s.ShopCode).ToList()
            };
            return PartialView("_ListShop", viewModel);
        }

        public ActionResult GetShopSuggestion(string shopName)
        {
            var shops = _shopService.GetShopSuggestion(shopName);
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
            if (_shopService.CheckExistShopName(viewModel.ShopName))
            {
                ModelState.AddModelError("ShopName", Messages.ShopExist);
            }

            if (ModelState.IsValid)
            {
                var criteria = GetShopCriteriaFromViewModal(viewModel);

                isSuccessful = _shopService.CreateNewShop(criteria);
                message = isSuccessful ? Messages.CreateShopSuccess : Messages.Error;
            }

            var errors = from item in ModelState
                         where item.Value.Errors.Any()
                         select new { item.Key, item.Value.Errors.FirstOrDefault().ErrorMessage };

            return Json(new { isSuccessful, message, errors });
        }

        [NapAuth(FunctionKey = FunctionKeys.ShopManagement, Policy = Constant.Policy.Host)]
        public ActionResult Edit(int id)
        {
            var shop = _shopService.GetShopById(id);
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
            var shop = _shopService.GetShopById(id);

            if (shop != null && shop.ShopName != viewModel.ShopName && _shopService.CheckExistShopName(viewModel.ShopName))
            {
                ModelState.AddModelError("ShopName", Messages.ShopExist);
            }

            if (ModelState.IsValid)
            {
                var criteria = GetShopCriteriaFromViewModal(viewModel);

                isSuccessful = _shopService.UpdateShop(id, criteria);
                message = isSuccessful ? Messages.UpdateShopSuccess : Messages.Error;
            }

            var errors = from item in ModelState
                         where item.Value.Errors.Any()
                         select new { item.Key, item.Value.Errors.FirstOrDefault().ErrorMessage };

            return Json(new { isSuccessful, message, errors });
        }

        private ShopCriteria GetShopCriteriaFromViewModal(ShopViewModel viewModel)
        {
            var criteria = new ShopCriteria
            {
                ShopName = viewModel.ShopName,
                ShopCode = viewModel.ShopCode,
                ContactName = viewModel.ContactName,
                ContactNumber = viewModel.ContactNumber,
                Email = viewModel.Email,
                Address = viewModel.Address,
                IsActive = viewModel.IsActive,
                ModifiedBy = CurrentSession.Username,
            };
            return criteria;
        }
    }
}