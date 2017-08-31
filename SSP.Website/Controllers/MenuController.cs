namespace SSP.Website.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using SSP.Core.Models;
    using SSP.Criteria.Menu;
    using SSP.Models.Menu;
    using SSP.Services;
    using SSP.Website.ViewModels.Menu;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class MenuController : BaseController
    {
        private readonly IMenuService menuService;
        private readonly IFunctionService functionService;

        public MenuController(
            IMenuService menuService,
            IFunctionService functionService)
        {
            this.menuService = menuService ?? new MenuService();
            this.functionService = functionService ?? new FunctionService();
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            IEnumerable<Menu> menus = await menuService.GetMenuAsync();

            return View(menus);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var viewModel = new MenuViewModel
            {
                Menus = await menuService.GetMenuAsync(),
                Functions = await functionService.GetFunctionAsync()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MenuViewModel menu)
        {
            if (ModelState.IsValid)
            {
                var criteria = new InsertMenuCriteria
                {
                    Name = menu.Name,
                    Link = menu.Link,
                    IsActive = menu.IsActive,
                    DisplayOrder = menu.DisplayOrder,
                    FunctionId = (!menu.FunctionId.HasValue || menu.FunctionId.Value <= 0) ? null : menu.FunctionId,
                    ParentId = (!menu.ParentId.HasValue || menu.ParentId.Value <= 0) ? null : menu.ParentId
                };

                CommandResult result = await menuService.InsertAsync(criteria);

                if (result.IsSuccessful)
                {
                    return RedirectToAction("Index");
                }
            }

            menu.Menus = await menuService.GetMenuAsync();
            menu.Functions = await functionService.GetFunctionAsync();

            return View(menu);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            Menu menu = await menuService.GetMenuByIdAsync(id);

            var viewModel = new MenuViewModel
            {
                MenuId = menu.MenuId,
                Name = menu.Name,
                Link = menu.Link,
                IsActive = menu.IsActive,
                DisplayOrder = menu.DisplayOrder,
                ParentId = menu.ParentId,
                FunctionId = menu.FunctionId,
                Menus = await menuService.GetMenuAsync(),
                Functions = await functionService.GetFunctionAsync()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(MenuViewModel menu)
        {
            if (ModelState.IsValid)
            {
                var criteria = new UpdateMenuCriteria
                {
                    MenuId = menu.MenuId,
                    Name = menu.Name,
                    Link = menu.Link,
                    DisplayOrder = menu.DisplayOrder,
                    IsActive = menu.IsActive,
                    FunctionId = (!menu.FunctionId.HasValue || menu.FunctionId.Value <= 0) ? null : menu.FunctionId,
                    ParentId = (!menu.ParentId.HasValue || menu.ParentId.Value <= 0) ? null : menu.ParentId
                };

                CommandResult result = await menuService.UpdateAsync(criteria);

                if (result.IsSuccessful)
                {
                    return RedirectToAction("Index");
                }
            }

            menu.Menus = await menuService.GetMenuAsync();
            menu.Functions = await functionService.GetFunctionAsync();

            return View(menu);
        }

        [HttpPost]
        public async Task<JsonResult> Delete(int id)
        {
            CommandResult result = await menuService.DeleteAsync(id);

            return Json(result);
        }
    }
}