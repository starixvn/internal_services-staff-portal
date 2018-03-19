namespace TicketDesk.Service.Implement
{
    using System.Collections.Generic;
    using Domain.Model;
    using Interface;

    public class TicketCategoryService : EntityFrameworkBaseService, ITicketCategoryService
    {
        public IEnumerable<TicketCategory> GetTicketCategories()
        {
            return context.TicketCategories;
        }
    }
}
