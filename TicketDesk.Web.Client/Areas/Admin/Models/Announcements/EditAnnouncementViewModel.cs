namespace TicketDesk.Web.Client.Areas.Admin.Models.Announcements
{
    using System.ComponentModel.DataAnnotations;
    using Core.Enumerations;
    using Domain.Model;
    using Localization.Client;

    public class EditAnnouncementViewModel
    {
        public Announcement Announcement { get; set; }

        [Display(Name = nameof(Resources.Important), ResourceType = typeof(Resources))]
        public bool IsImportant
        {
            get => Announcement.Type == AnnouncementType.Important;
            set => Announcement.Type = value ? AnnouncementType.Important : AnnouncementType.Normal;
        }

        public bool IsPublishedImmediately { get; set; }
    }
}