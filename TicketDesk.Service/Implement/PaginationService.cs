namespace TicketDesk.Service.Implement
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Dynamic;
    using System.Text;
    using Core.Model;
    using Interface;

    public class PaginationService<T, TResult> : IPaginationService<T, TResult>
    {
        public IEnumerable<T> Pagination(IEnumerable<T> originalList, Func<T, TResult> orderBy, int skipped, int took)
        {
            return originalList.AsEnumerable().OrderByDescending(orderBy).Skip(skipped).Take(took);
        }

        public IEnumerable<T> Pagination(IEnumerable<T> originalList, IList<SortData> sortDatas, int skipped, int took)
        {
            var result = originalList.AsEnumerable();

            var sortOrder = sortDatas.OrderBy(x => x.Priority);

            var orderByString = new StringBuilder();
            foreach (var sort in sortOrder)
            {
                orderByString.Append(sort.Column);
                orderByString.Append(sort.IsAsc ? " ascending," : " descending,");
            }

            return sortDatas.Any()
                ? result.OrderBy(orderByString.ToString().TrimEnd(',')).Skip(skipped).Take(took)
                : result.Skip(skipped).Take(took);
        }
    }
}