namespace TicketDesk.Web.Client
{
    using PagedList;
    using System.Linq;
    using System.Threading.Tasks;

    public static class PagedListAsyncExtensions
    {
        public static async Task<IPagedList<T>> ToPagedListAsync<T>(this IQueryable<T> superset, int pageNumber, int pageSize)
        {
            return await PagedListAsync<T>.Create(superset, pageNumber, pageSize).ConfigureAwait(false);
        }
    }
}