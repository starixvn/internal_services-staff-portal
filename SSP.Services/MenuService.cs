namespace SSP.Services
{
    using SSP.Core.Models;
    using SSP.Core.Services;
    using SSP.Criteria.Menu;
    using SSP.Models.Menu;
    using SSP.Repositories;
    using System.Collections.Generic;

    public class MenuService : BaseService, IMenuService
    {
        private readonly IMenuRepository menuRepository;

        public MenuService(IMenuRepository menuRepository = null)
        {
            this.menuRepository = menuRepository ?? new MenuRepository();
        }

        public IEnumerable<Menu> GetMenus()
        {
            var criteria = new GetMenuCriteria();
            IEnumerable<Menu> menus = new List<Menu>();

            if (criteria.IsValid())
            {
                menus = menuRepository.GetMenus(criteria);
            }

            return menus;
        }

        public Menu GetMenuById(int id)
        {
            var criteria = new GetMenuByIdCriteria { MenuId = id };
            var menu = new Menu();
            if (criteria.IsValid())
            {
                menu = GetDataEntity<Menu>(criteria);
            }

            return menu;
        }

        public CommandResult Insert(InsertMenuCriteria criteria)
        {
            var result = new CommandResult();
            if (criteria != null && criteria.IsValid())
            {
                result = Repository.ExecuteNonQuery(criteria);
            }

            return result;
        }

        public CommandResult Update(UpdateMenuCriteria criteria)
        {
            var result = new CommandResult();
            if (criteria != null && criteria.IsValid())
            {
                result = Repository.ExecuteNonQuery(criteria);
            }

            return result;
        }

        public CommandResult Delete(int id)
        {
            var criteria = new DeleteMenuCriteria { MenuId = id };
            var result = new CommandResult();
            if (criteria != null && criteria.IsValid())
            {
                result = Repository.ExecuteNonQuery(criteria);
            }

            return result;
        }
    }
}