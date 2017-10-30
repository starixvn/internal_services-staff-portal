namespace TicketDesk.Web.Client.Models.Ticket
{
    using Domain.Model;
    using System.Text.RegularExpressions;

    public class NewFeedbackTicketViewModel : TicketWithFilesBaseViewModel
    {
        private string[] _feedbackByValues;

        public Ticket Ticket { get; set; }

        public CustomFieldValue FeedbackBy { get; set; }

        public CustomFieldValue Subject { get; set; }

        public CustomFieldValue CustomerName { get; set; }

        public CustomFieldValue Feedback { get; set; }

        public string[] FeedbackByValues
        {
            get
            {
                if (_feedbackByValues != null)
                {
                    return _feedbackByValues;
                }

                _feedbackByValues = Regex.Split(FeedbackBy.CustomField.Values, "\r\n");
                return _feedbackByValues;
            }
        }

        public NewFeedbackTicketViewModel()
        {
            Ticket = new Ticket();
            FeedbackBy = new CustomFieldValue();
            Subject = new CustomFieldValue();
            CustomerName = new CustomFieldValue();
            Feedback = new CustomFieldValue();
        }
    }
}