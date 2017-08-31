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

        public async Task<IEnumerable<Menu>> GetMenuAsync()
        {
            var criteria = new GetMenuCriteria();
            IEnumerable<Menu> menus = new List<Menu>();

            if (criteria.IsValid())
            {
                menus = await menuRepository.GetMenuAsync(criteria);
            }

            return menus;
        }

        public async Task<Menu> GetMenuByIdAsync(int id)
        {
            var criteria = new GetMenuByIdCriteria { MenuId = id };
            var menu = new Menu();
            if (criteria.IsValid())
            {
                menu = await GetDataEntityAsync<Menu>(criteria);
            }

            return menu;
        }

        public async Task<CommandResult> InsertAsync(InsertMenuCriteria criteria)
        {
            var result = new CommandResult();
            if (criteria != null && criteria.IsValid())
            {
                result = await Repository.ExecuteNonQueryAsync(criteria);
            }

            return result;
        }

        public async Task<CommandResult> UpdateAsync(UpdateMenuCriteria criteria)
        {
            var result = new CommandResult();
            if (criteria != null && criteria.IsValid())
            {
                result = await Repository.ExecuteNonQueryAsync(criteria);
            }

            return result;
        }

        public async Task<CommandResult> DeleteAsync(int id)
        {
            var criteria = new DeleteMenuCriteria { MenuId = id };
            var result = new CommandResult();
            if (criteria != null && criteria.IsValid())
            {
                result = await Repository.ExecuteNonQueryAsync(criteria);
            }

            return result;
        }
    }
}