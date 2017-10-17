namespace TicketDesk.Web.Client.Models
{
    using Domain;
    using Domain.Model;
    using Identity;
    using PagedList;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    public class TicketCenterListViewModel
    {
        private FilterBarViewModel _filterBar;

        /// <summary>
        /// Initializes a new instance of the <see cref="TicketCenterListViewModel" /> class.
        /// </summary>
        private TicketCenterListViewModel()
        {
            DisplayProjects = false;
        }

        public int CurrentPage { get; set; }

        public bool DisplayProjects { get; set; }

        /// <summary>
        /// Gets or (private) sets the filter bar model.
        /// </summary>
        /// <value>The filter bar.</value>
        public FilterBarViewModel FilterBar => _filterBar ?? (_filterBar = new FilterBarViewModel(CurrentListSetting));

        /// <summary>
        /// Gets or (private) sets the list of tickets for the view.
        /// </summary>
        /// <value>The tickets.</value>
        public IPagedList<Domain.Model.Ticket> Tickets { get; private set; }

        public UserTicketListSetting CurrentListSetting { get; private set; }

        public IOrderedEnumerable<UserTicketListSetting> UserListSettings { get; private set; }

        public TicketDeskUserManager UserManager => DependencyResolver.Current.GetService<TicketDeskUserManager>();

        public static async Task<TicketCenterListViewModel> GetViewModelAsync(int currentPage, string listName, TdDomainContext context, string userId)
        {
            var userSettings = await context.UserSettingsManager.GetSettingsForUserAsync(userId);

            var vm = new TicketCenterListViewModel
            {
                UserListSettings = userSettings.ListSettings.OrderBy(lp => lp.ListMenuDisplayOrder),
                CurrentPage = currentPage,
                CurrentListSetting = userSettings.GetUserListSettingByName(listName)
            };

            vm.Tickets = await vm.ListTicketsAsync(currentPage, context, userId);

            var categories = context.TicketCategories.ToList();
            foreach (var ticket in vm.Tickets)
            {
                ticket.CategoryName =
                    categories.Where(x => x.CategoryId == ticket.CategoryId).Select(x => x.Name).FirstOrDefault();
            }

            return vm;
        }

        public async Task<IPagedList<Domain.Model.Ticket>> ListTicketsAsync(int pageIndex, TdDomainContext context, string napUserId)
        {
            var filterColumns = CurrentListSetting.FilterColumns.ToList();

            // if filtering by project, add filter for selected project
            var projectId = await context.UserSettingsManager.GetUserSelectedProjectIdAsync(context);
            if (projectId != default(int))
            {
                filterColumns.Add(new UserTicketListFilterColumn("ProjectId", true, projectId));
            }

            DisplayProjects = context.Projects.Count() > 1;

            var sortColumns = CurrentListSetting.SortColumns.ToList();
            var pageSize = CurrentListSetting.ItemsPerPage;
            var query = context.GetObjectQueryFor(context.Tickets);

            query = filterColumns.ApplyToQuery(query);
            query = sortColumns.ApplyToQuery(query);
            
            return await query.ToPagedListAsync(pageIndex, pageSize);
        }
    }
}