namespace SSP.Services
{
    using SSP.Core.Models;
    using SSP.Criteria.Menu;
    using SSP.Models.Menu;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IMenuService
    {
        Task<IEnumerable<Menu>> GetMenuAsync();

        Task<Menu> GetMenuByIdAsync(int id);

        Task<CommandResult> InsertAsync(InsertMenuCriteria criteria);

        Task<CommandResult> UpdateAsync(UpdateMenuCriteria criteria);

        Task<CommandResult> DeleteAsync(int id);
    }
}