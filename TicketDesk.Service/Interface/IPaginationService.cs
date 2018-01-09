namespace TicketDesk.Service.Interface
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public interface IPaginationService<T, TResult>
    {
        IEnumerable<T> Pagination(IQueryable<T> originalList, Func<T, TResult> orderBy, int skipped, int took);
    }
}
