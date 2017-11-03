namespace TicketDesk.Web.Client.Models
{
    using System.ComponentModel.DataAnnotations;
    using Ticket;

    public class IdUploaderViewModel : TicketWithFilesBaseViewModel
    {
        [Required]
        [Display(Name = "Customer's Username")]
        public string CustomerUserName { get; set; }

        [Required]
        [Display(Name = "Customer's Name")]
        public string CustomerName { get; set; }

        [Required]
        [Display(Name = "Id number")]
        public string IdNumber { get; set; }

        [Display(Name = "Remark")]
        public string Remark { get; set; }
    }
}