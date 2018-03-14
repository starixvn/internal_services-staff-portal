namespace TicketDesk.Web.Client.Areas.Admin.Models.Announcements
{
    using System.ComponentModel.DataAnnotations;
    using Criteria.Announcement;
    using Domain.Model;
    using Localization.Client;

    public class NewAnnouncementViewModel
    {
        public Announcement Announcement { get; set; }

        [Display(Name = nameof(Resources.Important), ResourceType = typeof(Resources))]
        public bool IsImportant { get; set; }

        public ModalId ModalId { get; set; } = ModalId.CreateAnnouncementModal;

        public ActionType ActionType { get; set; } = ActionType.Create;

        public bool IsPublishedImmediately { get; set; }

        public NewAnnouncementCriteria GetNewAnnouncementCriteria(string modifiedBy)
        {
            return new NewAnnouncementCriteria
            {
                Announcement = Announcement,
                IsImportant = IsImportant,
                ModifiedBy = modifiedBy
            };
        }
    }

    public enum ActionType
    {
        Create,

        [Display(Name = nameof(Resources.Sumbit), ResourceType = typeof(Resources))]
        Edit,

        Delete
    }

    public enum ModalId
    {
        [Display(Name = nameof(Resources.CreateNewAnnouncement), ResourceType = typeof(Resources))]
        CreateAnnouncementModal,

        [Display(Name = nameof(Resources.EditAnnouncement), ResourceType = typeof(Resources))]
        EditAnnouncementModal,

        [Display(Name = nameof(Resources.PublishAnnouncement), ResourceType = typeof(Resources))]
        PublishAnnouncement,

        [Display(Name = nameof(Resources.DeleteAnnouncement), ResourceType = typeof(Resources))]
        DeleteAnnouncementModal
    }
}