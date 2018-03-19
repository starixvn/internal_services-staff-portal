namespace TicketDesk.Service.Interface
{
    using System.Collections.Generic;
    using Domain.Model;

    public interface ITicketCategoryService
    {
        IEnumerable<TicketCategory> GetTicketCategories();
    }
}
