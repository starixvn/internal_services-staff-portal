namespace TicketDesk.Web.Client.Areas.Admin.Models.Announcements
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;
    using Client.Models.Common;
    using Core.Enumerations;
    using Domain.Model;

    public class AnnouncementViewModel : Pagination
    {
        public AnnouncementViewModel()
        {
            Action = "Index";
            Controller = "Announcements";
        }

        public IEnumerable<Announcement> Announcements { get; set; }

        [Display(Name = "Published From")]
        public DateTime? PublishedFrom { get; set; }

        [Display(Name = "Published To")]
        public DateTime? PublishedTo { get; set; }

        [Display(Name = "Status")]
        public AnnouncementStatus? Status { get; set; }

        [Display(Name = "State")]
        public AnnouncementState? State { get; set; }

        public override string IndexLink(int i, int itemPerPage)
        {
            var index = new AnnouncementViewModel
            {
                CurrentPage = i,
                ItemPerPage = itemPerPage,
                PublishedFrom = PublishedFrom,
                PublishedTo = PublishedTo,
                Status = Status,
                State = State
            };

            var routeValues = new RouteValueDictionary
            {
                { nameof(index.CurrentPage), index.CurrentPage },
                { nameof(index.ItemPerPage), index.ItemPerPage },
                { nameof(index.PublishedFrom), index.PublishedFrom },
                { nameof(index.PublishedTo), index.PublishedTo }
            };

            if (index.Status != null)
            {
                routeValues.Add(nameof(index.Status), (int)index.Status);
            }

            if (index.State != null)
            {
                routeValues.Add(nameof(index.State), (int)index.State);
            }

            return UrlHelper.GenerateUrl(
                RouteNames.AdminDefault.ToString(),
                Action,
                Controller,
                routeValues,
                RouteTable.Routes,
                HttpContext.Current.Request.RequestContext,
                false);
        }
    }
}