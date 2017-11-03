namespace TicketDesk.Domain.Model
{
    using System.ComponentModel.DataAnnotations;

    public class UploadedUserId
    {
        [Key]
        public int Id { get; set; }

        public string CustomerUsername { get; set; }

        public string CustomerName { get; set; }

        public string IdNumber { get; set; }

        public string Remark { get; set; }
    }
}