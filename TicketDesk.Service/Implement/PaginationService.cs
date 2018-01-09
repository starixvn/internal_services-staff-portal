namespace TicketDesk.Service.Implement
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class PaginationService<T, TResult>
    {
        public IEnumerable<T> Pagination(IQueryable<T> originalList, Func<T, TResult> function,  int skipped, int took)
        {
            return originalList.AsEnumerable().OrderByDescending(function).Skip(skipped).Take(took);
        }
    }
}
