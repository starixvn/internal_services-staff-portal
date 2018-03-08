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
                ActiveState = ActiveState.All
            };
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult GetListShop(ShopFilterViewModel viewModel)
        {
            var criteria = new ShopFilterCrietria
            {
                ShopId = viewModel.ShopId,
                ShopName = viewModel.ShopName,
                ActiveState = viewModel.ActiveState
            };

            var shops = _shopService.GetListShop(criteria);
            var shopList = shops.OrderBy(s => s.ShopCode).ToList();

            return PartialView("_ListShop", shopList);
        }

        public ActionResult GetShopSuggestion(string shopName)
        {
            var shops = _shopService.GetShopSuggestion(shopName);
            return Json(shops, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Create()
        {
            var viewModel = new ShopViewModel();
            return PartialView("_Create", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
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