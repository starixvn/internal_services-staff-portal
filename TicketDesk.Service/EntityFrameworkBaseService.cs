namespace TicketDesk.Service
{
    using System;
    using Domain;

    public class EntityFrameworkBaseService : IDisposable
    {
        protected readonly TdDomainContext context = new TdDomainContext();

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                context?.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
