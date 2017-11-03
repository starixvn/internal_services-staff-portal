namespace TicketDesk.Web.Client.Models.Ticket
{
    using System;

    public class TicketWithFilesBaseViewModel
    {
        public Guid TempId { get; }
        public int MaxBytes { get; set; }
        public string[] AcceptedExtensions { get; set; }
        public int MaxNumberOfFile { get; set; }

        public TicketWithFilesBaseViewModel()
        {
            TempId = Guid.NewGuid();
            MaxBytes = 33554432;
            AcceptedExtensions = new[] { ".jpg", ".jpeg", ".png", ".pdf" };
            MaxNumberOfFile = 20;
        }
    }
}