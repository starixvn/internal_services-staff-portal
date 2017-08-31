namespace SSP.Repositories
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using SSP.Models.Function;
    using SSP.Models.Menu;
    using System.Collections.Generic;

    public class MenuRepository : IMenuRepository
    {
        public IEnumerable<Menu> GetMenus(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                var menus = db.Query<Menu, Function, Menu, Menu>(
                    (menu, func, parent) =>
                    {
                        menu.Function = func;
                        menu.ParentMenu = parent;

                        return menu;
                    },
                    "FunctionId, MenuId",
                    criteria.GetSpParams());

                return menus;
            }
        }
    }
}