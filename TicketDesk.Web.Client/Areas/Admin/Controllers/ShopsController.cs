namespace TicketDesk.Web.Client.Areas.Admin.Controllers
{
    using System;
    using System.Linq;
    using System.Web.Mvc;
    using Client.Controllers;
    using Core.Enumerations;
    using Domain;
    using Domain.Infrastructure;
    using Domain.Model;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Web.Attribute;
    using Models.ShopManagement;

    [NapAuth(FunctionKey = FunctionKeys.ShopManagement, Policy = Constant.Policy.Read)]
    public class ShopsController : BaseController
    {
        private TdDomainContext Context { get; }

        public ShopsController(TdDomainContext context)
        {
            Context = context;
        }

        public ActionResult Index()
        {
            var viewModel = new ShopFilterViewModel
            {
                IsActive = ActiveState.All
            };
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult GetListShop(ShopFilterViewModel viewModel)
        {
            bool isActive = viewModel.IsActive == ActiveState.Yes;
            var shopList = viewModel.IsActive == ActiveState.All ? Context.Shops.ToList() : Context.Shops.Where(s => s.IsActive == isActive).ToList();

            return PartialView("_ListShop", shopList.OrderBy(s => s.LastUpdatedDate));
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
            if (IsExistedShopName(viewModel.ShopName))
            {
                ModelState.AddModelError("ShopName", "The Shop Name already exists.");
            }

            if (ModelState.IsValid)
            {
                var newShop = new Shop
                {
                    ShopName = viewModel.ShopName,
                    ShopCode = viewModel.ShopCode,
                    ContactName = viewModel.ContactName,
                    ContactNumber = viewModel.ContactNumber,
                    Email = viewModel.Email,
                    Address = viewModel.Address,
                    IsActive = viewModel.IsActive,
                    LastUpdatedBy = CurrentSession.Username,
                    LastUpdatedDate = new DateTime()
                };
                Context.Shops.Add(newShop);
                Context.SaveChanges();
                isSuccessful = true;
                message = "Create new shop successfully!";
            }

            var errors = from item in ModelState
                         where item.Value.Errors.Any()
                         select new { item.Key, item.Value.Errors.FirstOrDefault().ErrorMessage };

            return Json(new { isSuccessful, message, errors });
        }

        private bool IsExistedShopName(string shopName)
        {
            return Context.Shops.Where(x => x.ShopName == shopName).Any();
        }

        public ActionResult Edit(int id)
        {
            var shop = Context.Shops.Find(id);
            if (shop != null)
            {
                var viewModel = new ShopViewModel
                {
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
        public ActionResult Edit(int id, ShopViewModel model)
        {
            bool isSuccessful = false;
            string message = string.Empty;
            var shop = Context.Shops.Find(id);
            if (shop == null)
            {
                message = "Shop not exist";
            }

            if (IsExistedShopName(model.ShopName))
            {
                ModelState.AddModelError("ShopName", "Name existed!");
            }

            if (ModelState.IsValid && shop != null)
            {
                shop.ShopName = model.ShopName;
                shop.ShopCode = model.ShopCode;
                shop.ContactName = model.ContactName;
                shop.ContactNumber = model.ContactNumber;
                shop.Email = model.Email;
                shop.Address = model.Address;
                shop.IsActive = model.IsActive;
                shop.LastUpdatedBy = CurrentSession.Username;
                shop.LastUpdatedDate = new DateTime();
                Context.SaveChanges();
                isSuccessful = true;
            }

            var errors = from item in ModelState
                         where item.Value.Errors.Any()
                         select new { item.Key, item.Value.Errors.FirstOrDefault().ErrorMessage };

            return Json(new { isSuccessful, message, errors });
        }
    }
}