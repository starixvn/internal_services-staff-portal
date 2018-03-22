namespace TicketDesk.Service.Interface
{
    using System.Collections.Generic;
    using Domain.Model;

    public interface ITicketCategoryService
    {
        TicketCategory GetTicketCategory(int categoryId);

        IEnumerable<TicketCategory> GetTicketCategories();

        bool UpdateTicketCategory(TicketCategory ticketCategory);
    }
}
