namespace TicketDesk.Web.Client.Models.Home
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Core.Enumerations;
    using Criteria.Announcement;
    using Domain.Model;
    using Localization.Client;

    public class MoreAnnouncementViewModel
    {
        [Display(Name = nameof(Resources.PublishedForm), ResourceType = typeof(Resources))]
        public DateTime? PublishedFrom { get; set; }

        [Display(Name = nameof(Resources.PublishedTo), ResourceType = typeof(Resources))]
        public DateTime? PublishedTo { get; set; }

        public AnnouncementType? Type { get; set; }

        public IEnumerable<Announcement> Announcements { get; set; }

        public GetAnnouncementsCriteria GetAnnouncementsCriteria()
        {
            return new GetAnnouncementsCriteria
            {
                PublishedFrom = PublishedFrom,
                PublishedTo = PublishedTo,
                State = AnnouncementState.Published,
                Type = Type
            };
        }
    }
}