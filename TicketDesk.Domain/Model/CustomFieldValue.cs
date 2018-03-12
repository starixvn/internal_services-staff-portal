namespace TicketDesk.Domain.Model
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Web.Mvc;

    public class CustomFieldValue
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string GroupId { get; set; }

        [AllowHtml]
        public string Value { get; set; }

        public int TicketId { get; set; }

        public int CustomFieldId { get; set; }

        [NotMapped]
        public virtual CustomField CustomField { get; set; }

        public void AddDetails(string groupId, int ticketId)
        {
            GroupId = groupId;
            TicketId = ticketId;
        }
    }
}