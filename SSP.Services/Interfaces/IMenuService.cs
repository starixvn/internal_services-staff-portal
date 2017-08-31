namespace SSP.Services
{
    using SSP.Core.Models;
    using SSP.Criteria.Menu;
    using SSP.Models.Menu;
    using System.Collections.Generic;

    public interface IMenuService
    {
        IEnumerable<Menu> GetMenus();

        Menu GetMenuById(int id);

        CommandResult Insert(InsertMenuCriteria criteria);

        CommandResult Update(UpdateMenuCriteria criteria);

        CommandResult Delete(int id);
    }
}