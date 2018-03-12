namespace TicketDesk.Service
{
    using System;
    using System.Web.Mvc;
    using Domain;

    public class EntityFrameworkBaseService : IDisposable
    {
        protected readonly TdDomainContext context = DependencyResolver.Current.GetService<TdDomainContext>();

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
