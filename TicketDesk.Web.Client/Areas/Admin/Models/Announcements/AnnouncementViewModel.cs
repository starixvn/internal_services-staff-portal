namespace TicketDesk.Web.Client.Areas.Admin.Models.Announcements
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Core.Enumerations;
    using Criteria.Announcement;
    using Domain.Model;

    public class AnnouncementViewModel
    {
        public AnnouncementViewModel()
        {
            NewAnnouncementViewModel = new NewAnnouncementViewModel
            {
                Announcement = new Announcement(),
                IsImportant = false
            };
        }

        public IList<Announcement> Announcements { get; set; }

        public bool IsPublishImmediately { get; set; }

        [Display(Name = "Published From")]
        public DateTime? PublishedFrom { get; set; }

        [Display(Name = "Published To")]
        public DateTime? PublishedTo { get; set; }

        [Display(Name = "Type")]
        public AnnouncementType? Type { get; set; }

        [Display(Name = "Published")]
        public AnnouncementState? State { get; set; }

        // Create
        public NewAnnouncementViewModel NewAnnouncementViewModel { get; set; }

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