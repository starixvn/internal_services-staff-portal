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

        public bool UpdateTicketCategory(TicketCategory ticketCategory)
        {
            var originalTicketCategory = context.TicketCategories.Find(ticketCategory.CategoryId);

            if (originalTicketCategory != null)
            {
                originalTicketCategory.MailTo = ticketCategory.MailTo;
                originalTicketCategory.MailCcTo = ticketCategory.MailCcTo;
                originalTicketCategory.MailBccTo = ticketCategory.MailBccTo;
            }

            var result = context.SaveChanges();

            return result > 0;
        }
    }
}
