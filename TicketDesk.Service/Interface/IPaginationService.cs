namespace TicketDesk.Service.Interface
{
    using System;
    using System.Collections.Generic;
    using Core.Model;

    public interface IPaginationService<T, TResult>
    {
        IEnumerable<T> Pagination(IEnumerable<T> originalList, Func<T, TResult> orderBy, int skipped, int took);

        IEnumerable<T> Pagination(IEnumerable<T> originalList, IList<SortData> sortDatas, int skipped, int took);
    }
}