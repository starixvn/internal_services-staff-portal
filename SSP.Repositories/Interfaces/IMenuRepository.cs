namespace SSP.Repositories
{
    using Fanex.Data.Repository;
    using SSP.Models.Menu;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IMenuRepository
    {
        Task<IEnumerable<Menu>> GetMenuAsync(ICriteria criteria);
    }
}