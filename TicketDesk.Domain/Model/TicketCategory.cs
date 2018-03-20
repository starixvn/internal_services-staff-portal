namespace TicketDesk.Domain.Model
{
    using System.ComponentModel.DataAnnotations;

    public class TicketCategory
    {
        [Key]
        public int CategoryId { get; set; }

        public string Name { get; set; }

        public string Name_vi { get; set; }

        [Display(Name = "Mail To")]
        public string MailTo { get; set; }

        [Display(Name = "Cc Mail To")]
        public string MailCcTo { get; set; }

        [Display(Name = "Bcc Mail To")]
        public string MailBccTo { get; set; }
    }
}