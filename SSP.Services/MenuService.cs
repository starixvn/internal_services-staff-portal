namespace SSP.Services
{
    using SSP.Core.Models;
    using SSP.Core.Services;
    using SSP.Criteria.Menu;
    using SSP.Models.Menu;
    using SSP.Repositories;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class MenuService : BaseService, IMenuService
    {
        private readonly IMenuRepository menuRepository;

        public MenuService(IMenuRepository menuRepository = null)
        {
            this.menuRepository = menuRepository ?? new MenuRepository();
        }

        public async Task<IEnumerable<Menu>> GetMenus()
        {
            var criteria = new GetMenuCriteria();
            IEnumerable<Menu> menus = new List<Menu>();

            if (criteria.IsValid())
            {
                menus = await menuRepository.GetMenus(criteria);
            }

            return menus;
        }

        public async Task<Menu> GetMenuById(int id)
        {
            var criteria = new GetMenuByIdCriteria { MenuId = id };
            var menu = new Menu();
            if (criteria.IsValid())
            {
                menu = await GetDataEntity<Menu>(criteria);
            }

            return menu;
        }

        public async Task<CommandResult> Insert(InsertMenuCriteria criteria)
        {
            var result = new CommandResult();
            if (criteria != null && criteria.IsValid())
            {
                result = await Repository.ExecuteNonQuery(criteria);
            }

            return result;
        }

        public async Task<CommandResult> Update(UpdateMenuCriteria criteria)
        {
            var result = new CommandResult();
            if (criteria != null && criteria.IsValid())
            {
                result = await Repository.ExecuteNonQuery(criteria);
            }

            return result;
        }

        public async Task<CommandResult> Delete(int id)
        {
            var criteria = new DeleteMenuCriteria { MenuId = id };
            var result = new CommandResult();
            if (criteria != null && criteria.IsValid())
            {
                result = await Repository.ExecuteNonQuery(criteria);
            }

            return result;
        }
    }
}