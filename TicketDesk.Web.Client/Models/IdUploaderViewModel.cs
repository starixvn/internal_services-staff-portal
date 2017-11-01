namespace TicketDesk.Web.Client.Models
{
    using Infrastructure.Attribute;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Web;

    public class IdUploaderViewModel
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

        [FileType(new[] { "jpg", "jpeg", "png" })]
        [NumberOfFile(Max = 3)]
        public IEnumerable<HttpPostedFileBase> Files { get; set; }
    }
}