namespace TicketDesk.Domain.Model
{
    using System.ComponentModel.DataAnnotations;
    using TicketDesk.Localization.Client;

    public class TicketCategory
    {
        [Key]
        public int CategoryId { get; set; }

        public string Name { get; set; }

        public string Name_vi { get; set; }

        [Display(Name = nameof(Resources.MailTo), ResourceType = typeof(Resources))]
        public string MailTo { get; set; }

        [Display(Name = nameof(Resources.MailCc), ResourceType = typeof(Resources))]
        public string MailCcTo { get; set; }

        [Display(Name = nameof(Resources.MailBcc), ResourceType = typeof(Resources))]
        public string MailBccTo { get; set; }

        [Display(Name = "Allow additional mail")]
        public bool IsAllowAdditionalMail { get; set; }
    }
}