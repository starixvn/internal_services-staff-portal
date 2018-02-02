namespace TicketDesk.Domain.Model
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class NapUserSetting
    {
        [Key]
        public int UserId { get; set; }

        public string Settings { get; set; }

        public DateTime ModifiedDate { get; set; }

        public int ModifiedBy { get; set; }
    }
}
