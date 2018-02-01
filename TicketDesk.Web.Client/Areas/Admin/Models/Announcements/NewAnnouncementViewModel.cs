namespace TicketDesk.Web.Client.Areas.Admin.Models.Announcements
{
    using System.ComponentModel.DataAnnotations;
    using Criteria.Announcement;
    using Domain.Model;

    public class NewAnnouncementViewModel
    {
        public Announcement Announcement { get; set; }

        [Display(Name = "Important")]
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

        [Display(Name = "Sumbit")]
        Edit,

        Delete
    }

    public enum ModalId
    {
        [Display(Name = "Create New Announcement")]
        CreateAnnouncementModal,

        [Display(Name = "Edit Announcement")]
        EditAnnouncementModal,

        [Display(Name = "Publish Announcement")]
        PublishAnnouncement,

        [Display(Name = "Delete Announcement")]
        DeleteAnnouncementModal
    }
}