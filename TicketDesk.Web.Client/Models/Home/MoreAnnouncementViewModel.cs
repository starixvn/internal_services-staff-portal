namespace TicketDesk.Web.Client.Models.Home
{
    using System;
    using System.Collections.Generic;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;
    using Common;
    using Core.Enumerations;
    using Criteria.Announcement;
    using Domain.Model;

    public class MoreAnnouncementViewModel : Pagination
    {
        public MoreAnnouncementViewModel()
        {
            Action = "MoreAnnouncement";
            Controller = "Home";
        }

        public DateTime? PublishedFrom { get; set; }

        public DateTime? PublishedTo { get; set; }

        public IList<Announcement> Announcements { get; set; }

        public override string IndexLink(int i, int itemPerPage)
        {
            var index = new MoreAnnouncementViewModel
            {
                CurrentPage = i,
                ItemPerPage = itemPerPage,
                PublishedFrom = PublishedFrom,
                PublishedTo = PublishedTo
            };

            var routeValues = new RouteValueDictionary
            {
                { nameof(index.CurrentPage), index.CurrentPage },
                { nameof(index.ItemPerPage), index.ItemPerPage },
                { nameof(index.PublishedFrom), index.PublishedFrom },
                { nameof(index.PublishedTo), index.PublishedTo }
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

        public GetAnnouncementsCriteria GetAnnouncementsCriteria()
        {
            return new GetAnnouncementsCriteria
            {
                PublishedFrom = PublishedFrom,
                PublishedTo = PublishedTo,
                State = AnnouncementState.Published
            };
        }
    }
}