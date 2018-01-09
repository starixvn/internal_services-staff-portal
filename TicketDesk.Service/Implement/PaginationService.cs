namespace TicketDesk.Service.Implement
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Interface;

    public class PaginationService<T, TResult> : IPaginationService<T, TResult>
    {
        public IEnumerable<T> Pagination(IQueryable<T> originalList, Func<T, TResult> orderBy,  int skipped, int took)
        {
            return originalList.AsEnumerable().OrderByDescending(orderBy).Skip(skipped).Take(took);
        }
    }
}