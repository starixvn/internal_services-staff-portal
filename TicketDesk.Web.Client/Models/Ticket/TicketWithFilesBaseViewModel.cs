namespace TicketDesk.Web.Client.Models.Ticket
{
    using System;

    public class TicketWithFilesBaseViewModel
    {
        public Guid TempId { get; set; }
        public int MaxBytes { get; }
        public string[] AcceptedExtensions { get; }
        public int MaxNumberOfFile { get; }

        public TicketWithFilesBaseViewModel()
        {
            TempId = new Guid();
            MaxBytes = 33554432;
            AcceptedExtensions = new[] { ".jpg", ".jpeg", ".png", ".pdf" };
            MaxNumberOfFile = 20;
        }
    }
}