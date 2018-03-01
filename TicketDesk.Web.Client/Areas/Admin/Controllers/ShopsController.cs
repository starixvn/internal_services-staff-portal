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

        public ActionResult GetListShop(ShopFilterViewModel viewModel)
        {
            return PartialView("_ListShop");
        }

        public ActionResult Create()
        {
            var viewModel = new Shop();
            return PartialView("_Create", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Shop viewModel)
        {
            bool isSuccessful = false;
            if (IsExistedShopName(viewModel.ShopName))
            {
                ModelState.AddModelError("ShopName", "Name existed!");
            }

            if (ModelState.IsValid)
            {
                viewModel.LastUpdatedBy = CurrentSession.Username;
                viewModel.LastUpdatedDate = new DateTime();
                context.Shops.Add(viewModel);
                context.SaveChanges();
                isSuccessful = true;
            }

            return Json(new { isSuccessful, message = "Successful" });
        }

        private bool IsExistedShopName(string shopName)
        {
            return Context.Shops.Where(x => x.ShopName == shopName).Any();
        }

        public ActionResult Edit(int id)
        {
            var viewModel = Context.Shops.Find(id);
            if (viewModel != null)
            {
                return PartialView("_Edit", viewModel);
            }

            return HttpNotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Shop model)
        {
            bool isSuccessful = false;
            var message = string.Empty;
            var shop = Context.Shops.Find(id);
            if (shop == null)
            {
                message = "Shop not exist";
            }

            if (IsExistedShopName(model.ShopName))
            {
                ModelState.AddModelError("ShopName", "Name existed!");
            }

            if (ModelState.IsValid)
            {
                model.LastUpdatedBy = CurrentSession.Username;
                model.LastUpdatedDate = new DateTime();
                Context.Entry(shop).CurrentValues.SetValues(model);
                Context.SaveChanges();
                isSuccessful = true;
            }

            return Json(new { isSuccessful, message });
        }
    }
}