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
            IEnumerable<Menu> menus = await menuService.GetMenus();

            return View(menus);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var viewModel = new MenuViewModel
            {
                Menus = await menuService.GetMenus(),
                Functions = await functionService.GetFunctions()
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

                CommandResult result = await menuService.Insert(criteria);

                if (result.IsSuccessful)
                {
                    return RedirectToAction("Index");
                }
            }

            menu.Menus = await menuService.GetMenus();
            menu.Functions = await functionService.GetFunctions();

            return View(menu);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            Menu menu = await menuService.GetMenuById(id);

            var viewModel = new MenuViewModel
            {
                MenuId = menu.MenuId,
                Name = menu.Name,
                Link = menu.Link,
                IsActive = menu.IsActive,
                DisplayOrder = menu.DisplayOrder,
                ParentId = menu.ParentId,
                FunctionId = menu.FunctionId,
                Menus = await menuService.GetMenus(),
                Functions = await functionService.GetFunctions()
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

                CommandResult result = await menuService.Update(criteria);

                if (result.IsSuccessful)
                {
                    return RedirectToAction("Index");
                }
            }

            menu.Menus = await menuService.GetMenus();
            menu.Functions = await functionService.GetFunctions();

            return View(menu);
        }

        [HttpPost]
        public async Task<JsonResult> Delete(int id)
        {
            CommandResult result = await menuService.Delete(id);

            return Json(result);
        }
    }
}