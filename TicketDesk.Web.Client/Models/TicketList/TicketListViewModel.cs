namespace TicketDesk.Web.Client.Models.TicketList
{
    using System.Collections.Generic;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;
    using Common;
    using Core.Enumerations;
    using Criteria.TicketList;
    using Domain.Model;

    public class TicketListViewModel : Pagination
    {
        public TicketListViewModel()
        {
            Action = "Index";
            Controller = "Home";
            CurrentPage = 1;
            ItemPerPage = 20;
            Tickets = new List<TicketViewModel>();
        }

        public IEnumerable<TicketViewModel> Tickets { get; set; }

        public TicketStatus? Status { get; set; }

        public override string IndexLink(int i, int itemPerPage)
        {
            var ticketListViewModel = new TicketListViewModel
            {
                CurrentPage = i,
                ItemPerPage = itemPerPage
            };

            var routeValues = new RouteValueDictionary
            {
                { $"{nameof(ticketListViewModel.CurrentPage)}", ticketListViewModel.CurrentPage },
                { $"{nameof(ticketListViewModel.ItemPerPage)}", ticketListViewModel.ItemPerPage }
            };

            return UrlHelper.GenerateUrl(
                RouteNames.Default.ToString(),
                Action,
                Controller,
                routeValues,
                RouteTable.Routes,
                HttpContext.Current.Request.RequestContext,
                false);
        }
    }
}