namespace SSP.Repositories
{
    using Fanex.Data.Repository;
    using SSP.Models.Menu;
    using System.Collections.Generic;

    public interface IMenuRepository
    {
        IEnumerable<Menu> GetMenus(ICriteria criteria);
    }
}