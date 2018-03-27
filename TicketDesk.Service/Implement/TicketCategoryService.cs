namespace TicketDesk.Service.Implement
{
    using System.Collections.Generic;
    using System.Data.Entity;
    using Domain.Model;
    using Interface;

    public class TicketCategoryService : EntityFrameworkBaseService, ITicketCategoryService
    {
        public TicketCategory GetTicketCategory(int categoryId)
        {
            return context.TicketCategories.Find(categoryId);
        }

        public IEnumerable<TicketCategory> GetTicketCategories()
        {
            return context.TicketCategories;
        }

        public bool UpdateTicketCategory(TicketCategory ticketCategory)
        {
            var entry = context.Entry(ticketCategory);

            if (entry.State == EntityState.Detached)
            {
                context.TicketCategories.Attach(ticketCategory);

                entry.Property(x => x.MailTo).IsModified = true;
                entry.Property(x => x.MailCcTo).IsModified = true;
                entry.Property(x => x.MailBccTo).IsModified = true;
                entry.Property(x => x.IsAllowAdditionalMail).IsModified = true;
            }

            var result = context.SaveChanges();

            return result > 0;
        }
    }
}
