
namespace TicketDesk.Domain.Model
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class TicketCategoryCustomField
    {
        [Key]
        public int Id { get; set; }

        public int TicketCategory_CategoryId { get; set; }

        public int CustomField_Id { get; set; }


    }
}
