namespace SSP.Services
{
    using SSP.Core.Models;
    using SSP.Criteria.Menu;
    using SSP.Models.Menu;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IMenuService
    {
        Task<IEnumerable<Menu>> GetMenus();

        Task<Menu> GetMenuById(int id);

        Task<CommandResult> Insert(InsertMenuCriteria criteria);

        Task<CommandResult> Update(UpdateMenuCriteria criteria);

        Task<CommandResult> Delete(int id);
    }
}