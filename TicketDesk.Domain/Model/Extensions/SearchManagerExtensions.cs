////namespace TicketDesk.Domain.Model
////{
    ////using Search.Common;
    ////using System.Collections.Generic;
    ////using System.Linq;
    ////using System.Threading.Tasks;

    ////public static class SearchManagerExtensions
    ////{
    ////    public static async Task<IEnumerable<Ticket>> SearchAsync(
    ////        this TdSearchContext manager,
    ////        IQueryable<Ticket> ticketQuery,
    ////        string searchText,
    ////        int projectId)
    ////    {
    ////        var results = await manager.IndexSearcher.SearchAsync(searchText, projectId).ConfigureAwait(false);
    ////        var inFilter = results.Select(r => r.Id).ToList();

    ////        return from t in
    ////                   (from t in ticketQuery
    ////                    where inFilter.Contains(t.TicketId)
    ////                    select t).ToList()
    ////               join i in results
    ////               on t.TicketId equals i.Id
    ////               orderby i.SearchScore descending
    ////               select t;
    ////    }
    ////}
////}