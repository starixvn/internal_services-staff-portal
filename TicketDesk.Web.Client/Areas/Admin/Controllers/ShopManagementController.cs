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
    public class ShopManagementController : BaseController
    {
        private TdDomainContext Context { get; }

        public ShopManagementController(TdDomainContext context)
        {
            Context = context;
        }

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

        public ActionResult Create()
        {
            var viewModel = new ShopManagement();
            return PartialView("_Create", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ShopManagement viewModel)
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
                context.ShopManagement.Add(viewModel);
                context.SaveChanges();
                isSuccessful = true;
            }

            return Json(new { isSuccessful, message = "Successful" });
        }

        private bool IsExistedShopName(string shopName)
        {
            return Context.ShopManagement.Where(x => x.ShopName == shopName).Any();
        }

        public ActionResult Edit(int id)
        {
            var viewModel = Context.ShopManagement.Find(id);
            if (viewModel != null)
            {
                return PartialView("_Edit", viewModel);
            }

            return HttpNotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, ShopManagement model)
        {
            bool isSuccessful = false;
            var message = string.Empty;
            var shop = Context.ShopManagement.Find(id);
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