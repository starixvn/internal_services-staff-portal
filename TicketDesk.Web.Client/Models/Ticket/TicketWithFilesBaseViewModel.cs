namespace TicketDesk.Web.Client.Models.Ticket
{
    public class TicketWithFilesBaseViewModel
    {
        public int MaxBytes { get; }
        public string[] AcceptedExtensions { get; }
        public int MaxNumberOfFile { get; }

        public TicketWithFilesBaseViewModel()
        {
            MaxBytes = 33554432;
            AcceptedExtensions = new[] { ".jpg", ".jpeg", ".png", ".pdf" };
            MaxNumberOfFile = 20;
        }
    }
}