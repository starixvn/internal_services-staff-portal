namespace TicketDesk.Web.Client.Models.Home
{
    using System.Collections.Generic;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;
    using Common;
    using Core.Enumerations;
    using Domain.Model;

    public class MyOpenningTicketsViewModel
    {
        public MyOpenningTicketsViewModel()
        {
            ////Action = "Index";
            ////Controller = "Home";
            ////CurrentPage = 1;
            ////ItemPerPage = 20;
            TicketGroups = new List<TicketGroup>();
        }

        public IList<TicketGroup> TicketGroups { get; set; }

        ////public override string IndexLink(int i, int itemPerPage)
        ////{

        ////    var myOpenningTicketsViewModel = new MyOpenningTicketsViewModel
        ////    {
        ////        CurrentPage = i,
        ////        ItemPerPage = itemPerPage
        ////    };

        ////    var routeValues = new RouteValueDictionary
        ////    {
        ////        { $"{nameof(myOpenningTicketsViewModel.CurrentPage)}", myOpenningTicketsViewModel.CurrentPage },
        ////        { $"{nameof(myOpenningTicketsViewModel.ItemPerPage)}", myOpenningTicketsViewModel.ItemPerPage }
        ////    };

        ////    return UrlHelper.GenerateUrl(
        ////        RouteNames.Default.ToString(),
        ////        Action,
        ////        Controller,
        ////        routeValues,
        ////        RouteTable.Routes,
        ////        HttpContext.Current.Request.RequestContext,
        ////        false);
        ////}
    }
}