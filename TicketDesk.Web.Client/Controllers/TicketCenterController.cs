namespace TicketDesk.Web.Client.Controllers
{
    using System.Data.Entity;
    using Domain;
    using Domain.Infrastructure;
    using Domain.Model;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Web.Attribute;
    using Models;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    [RoutePrefix("tickets")]
    [Route("{action=index}")]
    [NapAuth(FunctionKey = FunctionKeys.TicketCenter, Policy = Constant.Policy.Read)]
    public class TicketCenterController : BaseController
    {
        public TicketCenterController(TdDomainContext context)
        {
            Context = context;
        }

        private TdDomainContext Context { get; }

        [Route("reset-user-lists")]
        public async Task<ActionResult> ResetUserLists()
        {
            var uId = Context.SecurityProvider.CurrentUserId;
            await Context.UserSettingsManager.ResetAllListSettingsForUserAsync(uId).ConfigureAwait(false);
            await Context.SaveChangesAsync().ConfigureAwait(false);
            return RedirectToAction("Index");
        }

        // GET: TicketCenter
        [Route("{listName?}/{page:int?}")]
        public async Task<ActionResult> Index(int? page, string listName)
        {
            // Role
            var names = listName ?? (Context.SecurityProvider.NapContextHelper.HasRight(FunctionKeys.ListAssignedTicket,
                            Constant.Policy.Read)
                            ? "unassigned"
                            : "mytickets");
            var pageNumber = page ?? 1;

            var viewModel = await TicketCenterListViewModel.GetViewModelAsync(pageNumber, names, Context,
                Context.SecurityProvider.CurrentUserId).ConfigureAwait(false);

            return View(viewModel);
        }

        [Route("{id:int}")]
        [NapAuth(FunctionKey = FunctionKeys.TicketCenter, Policy = Constant.Policy.Read)]
        public async Task<ActionResult> Index(int id)
        {
            var model = await Context.Tickets.Include(t => t.TicketSubscribers).FirstOrDefaultAsync(t => t.TicketId == id).ConfigureAwait(false);
            if (model == null)
            {
                return RedirectToAction("Index", "TicketCenter");
            }

            ViewBag.IsEditorDefaultHtml = Context.TicketDeskSettings.ClientSettings.GetDefaultTextEditorType() == "summernote";

            return View("SingleTicket", model);
        }

        [Route("pageList/{listName=mytickets}/{page:int?}")]
        public async Task<ActionResult> PageList(int? page, string listName)
        {
            return await GetTicketListPartial(page, listName).ConfigureAwait(false);
        }

        [Route("filterList/{listName=opentickets}/{page:int?}")]
        public async Task<PartialViewResult> FilterList(
            string listName,
            int pageSize,
            string ticketStatus,
            string owner,
            string assignedTo)
        {
            var uId = Context.SecurityProvider.CurrentUserId;
            var userSetting = await Context.UserSettingsManager.GetSettingsForUserAsync(uId).ConfigureAwait(false);

            var currentListSetting = userSetting.GetUserListSettingByName(listName);

            currentListSetting.ModifyFilterSettings(pageSize, ticketStatus, owner, assignedTo);

            await Context.SaveChangesAsync().ConfigureAwait(false);

            return await GetTicketListPartial(null, listName).ConfigureAwait(false);
        }

        [Route("sortList/{listName=opentickets}/{page:int?}")]
        public async Task<PartialViewResult> SortList(
            int? page,
            string listName,
            string columnName,
            bool isMultiSort = false)
        {
            var uId = Context.SecurityProvider.CurrentUserId;
            var userSetting = await Context.UserSettingsManager.GetSettingsForUserAsync(uId).ConfigureAwait(false);
            var currentListSetting = userSetting.GetUserListSettingByName(listName);

            var sortCol = currentListSetting.SortColumns.SingleOrDefault(sc => sc.ColumnName == columnName);

            if (isMultiSort)
            {
                // column already in sort, remove from sort
                if (sortCol != null)
                {
                    // only remove if there are more than one sort
                    if (currentListSetting.SortColumns.Count > 1)
                    {
                        currentListSetting.SortColumns.Remove(sortCol);
                    }
                }
                else
                {
                    // column not in sort, add to sort
                    currentListSetting.SortColumns.Add(new UserTicketListSortColumn(columnName,
                        ColumnSortDirection.Ascending));
                }
            }
            else
            {
                // column already in sort, just flip direction
                if (sortCol != null)
                {
                    sortCol.SortDirection = (sortCol.SortDirection == ColumnSortDirection.Ascending)
                        ? ColumnSortDirection.Descending
                        : ColumnSortDirection.Ascending;
                }
                else
                {
                    // column not in sort, replace sort with new simple sort for column
                    currentListSetting.SortColumns.Clear();
                    currentListSetting.SortColumns.Add(new UserTicketListSortColumn(columnName,
                        ColumnSortDirection.Ascending));
                }
            }

            await Context.SaveChangesAsync().ConfigureAwait(false);

            return await GetTicketListPartial(page, listName).ConfigureAwait(false);
        }

        private async Task<PartialViewResult> GetTicketListPartial(int? page, string listName)
        {
            var pageNumber = page ?? 1;

            var viewModel = await TicketCenterListViewModel
                .GetViewModelAsync(pageNumber, listName, Context, Context.SecurityProvider.CurrentUserId)
                .ConfigureAwait(false);
            return PartialView("_TicketList", viewModel);
        }
    }
}