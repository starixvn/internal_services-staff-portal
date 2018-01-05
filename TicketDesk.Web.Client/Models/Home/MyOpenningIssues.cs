namespace TicketDesk.Web.Client.Models.Home
{
    using System.Collections.Generic;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;
    using Common;

    public class MyOpenningIssues : Pagination
    {
        public MyOpenningIssues()
        {
            Action = "Index";
            Controller = "Home";
            CurrentPage = 1;
            ItemPerPage = 20;
            TicketGroups = new List<TicketGroup>();
        }

        public IList<TicketGroup> TicketGroups { get; set; }

        public override string NextLink()
        {
            return IndexLink(CurrentPage + 1, ItemPerPage);
        }

        public override string PreviousLink()
        {
            return IndexLink(CurrentPage - 1, ItemPerPage);
        }

        public override string IndexLink(int i, int itemPerPage)
        {
            var homeViewModel = new HomeViewModel
            {
                MyOpenningIssues = new MyOpenningIssues
                {
                    CurrentPage = i,
                    ItemPerPage = itemPerPage
                }
            };

            var routeValues = new RouteValueDictionary
            {
                { $"{nameof(homeViewModel.MyOpenningIssues)}.{nameof(homeViewModel.MyOpenningIssues.CurrentPage)}", homeViewModel.MyOpenningIssues.CurrentPage },
                { $"{nameof(homeViewModel.MyOpenningIssues)}.{nameof(homeViewModel.MyOpenningIssues.ItemPerPage)}", homeViewModel.MyOpenningIssues.ItemPerPage }
            };

            return UrlHelper.GenerateUrl(
                "Default",
                Action,
                Controller,
                routeValues,
                RouteTable.Routes,
                HttpContext.Current.Request.RequestContext,
                false);
        }

        public override string PageSizeLink(int itemPerPage)
        {
            return IndexLink(1, itemPerPage);
        }
    }
}