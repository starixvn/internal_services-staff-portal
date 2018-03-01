namespace TicketDesk.Domain.Model
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class CustomFieldListValues
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int CustomFieldId { get; set; }

        public string DisplayName { get; set; }

        public string DisplayName_vi { get; set; }

        public string Description { get; set; }

        public bool IsActive { get; set; }
    }
}
