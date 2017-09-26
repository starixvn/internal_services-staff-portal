namespace TicketDesk.Web.Client
{
    using PagedList;
    using System;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using TicketDesk.Localization.Infrastructure;

    public class PagedListAsync<T> : BasePagedList<T>
    {
        private PagedListAsync()
        {
        }

        public static async Task<IPagedList<T>> Create(IQueryable<T> superset, int pageNumber, int pageSize)
        {
            var list = new PagedListAsync<T>();
            await list.InitAsync(superset, pageNumber, pageSize);
            return list;
        }

        private async Task InitAsync(IQueryable<T> superset, int pageNumber, int pageSize)
        {
            if (pageNumber < 1)
            {
                throw new ArgumentOutOfRangeException("pageNumber", pageNumber, Strings.PageNumberBelow);
            }

            if (pageSize < 1)
            {
                throw new ArgumentOutOfRangeException("pageSize", pageSize, Strings.PageSizeLess);
            }

            TotalItemCount = superset == null ? 0 : await superset.CountAsync();
            PageSize = pageSize;
            PageNumber = pageNumber;
            PageCount = TotalItemCount > 0 ? (int)Math.Ceiling(TotalItemCount / (double)PageSize) : 0;
            HasPreviousPage = PageNumber > 1;
            HasNextPage = PageNumber < PageCount;
            IsFirstPage = PageNumber == 1;
            IsLastPage = PageNumber >= PageCount;
            FirstItemOnPage = ((PageNumber - 1) * PageSize) + 1;
            var num = FirstItemOnPage + PageSize - 1;
            LastItemOnPage = num > TotalItemCount ? TotalItemCount : num;
            if (superset == null || TotalItemCount <= 0)
            {
                return;
            }

            Subset.AddRange(pageNumber == 1
                ? await superset.Skip(0).Take(pageSize).ToListAsync()
                : await superset.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync());
        }
    }
}