namespace TicketDesk.Web.Client.Areas.Admin.Models.Announcements
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Core.Enumerations;
    using Criteria.Announcement;
    using Domain.Model;
    using Localization.Client;

    public class AnnouncementViewModel
    {
        public IList<Announcement> Announcements { get; set; }

        [Display(Name = nameof(Resources.PublishedFrom), ResourceType = typeof(Resources))]
        public DateTime? PublishedFrom { get; set; }

        [Display(Name = nameof(Resources.PublishedTo), ResourceType = typeof(Resources))]
        public DateTime? PublishedTo { get; set; }

        [Display(Name = nameof(Resources.Type), ResourceType = typeof(Resources))]
        public AnnouncementType? Type { get; set; }

        [Display(Name = nameof(Resources.Published), ResourceType = typeof(Resources))]
        public AnnouncementState? State { get; set; }

        public bool HaveHostPermission { get; set; }

        public GetAnnouncementsCriteria GetQueryCriteria()
        {
            return new GetAnnouncementsCriteria
            {
                PublishedFrom = PublishedFrom,
                PublishedTo = PublishedTo,
                Type = Type,
                State = State
            };
        }
    }
}