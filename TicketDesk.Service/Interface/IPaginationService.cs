namespace TicketDesk.Service.Interface
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Core.Model;

    public interface IPaginationService<T, TResult>
    {
        IEnumerable<T> Pagination(IQueryable<T> originalList, Func<T, TResult> orderBy, int skipped, int took);

        IEnumerable<T> Pagination(IQueryable<T> originalList, IList<SortData> sortDatas, int skipped, int took);
    }
}