namespace TicketDesk.Domain.Model
{
    using System.ComponentModel.DataAnnotations;

    public class TicketCategory
    {
        [Key]
        public int CategoryId { get; set; }

        public string Name { get; set; }

        public string Name_vi { get; set; }

        public string MailTo { get; set; }

        public string MailCcTo { get; set; }

        public string MailBccTo { get; set; }
    }
}