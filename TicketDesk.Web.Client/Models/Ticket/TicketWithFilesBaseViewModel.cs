namespace TicketDesk.Web.Client.Models.Ticket
{
    using System;

    public class TicketWithFilesBaseViewModel
    {
        public Guid TempId { get; set; }
        public int MaxBytes { get; set; }
        public string[] AcceptedExtensions { get; set; }
        public int MaxNumberOfFile { get; set; }
        public bool IsFileRequired { get; set; }

        public TicketWithFilesBaseViewModel()
        {
            MaxBytes = 33554432;
            AcceptedExtensions = new[] { ".jpg", ".jpeg", ".png", ".pdf" };
            MaxNumberOfFile = 20;
            IsFileRequired = false;
        }
    }
}