namespace TicketDesk.Web.Client.Models
{
    using System.ComponentModel.DataAnnotations;
    using Ticket;

    public class IdUploaderViewModel : TicketWithFilesBaseViewModel
    {
        [Required(ErrorMessage = "Customer Username is required!")]
        [StringLength(maximumLength: 50, ErrorMessage = "Customer Username should not less than 2 and more than 50 characters", MinimumLength = 2)]
        [RegularExpression("^[A-Za-z0-9._-]+", ErrorMessage = "Customer Username should not contains special characters")]
        [Display(Name = "Customer's Username")]
        public string CustomerUserName { get; set; }

        [Required(ErrorMessage = "Customer's Name is required!")]
        [StringLength(maximumLength: 50, ErrorMessage = "Customer's Name should not less than 2 and more than 50 characters", MinimumLength = 2)]
        [RegularExpression("[^$-&(-,:-?{-~!\\.\"^`@#/\\\\\\[\\]]+", ErrorMessage = "Customer's Name should not contains special characters")]
        [Display(Name = "Customer's Name")]
        public string CustomerName { get; set; }

        [Required(ErrorMessage = "ID Number is required!")]
        [StringLength(maximumLength: 12, ErrorMessage = "ID Number is not correct", MinimumLength = 9)]
        [RegularExpression("\\d+", ErrorMessage = "ID Number should not contains any character")]
        [Display(Name = "Id number")]
        public string IdNumber { get; set; }

        [Display(Name = "Remark")]
        public string Remark { get; set; }
    }
}