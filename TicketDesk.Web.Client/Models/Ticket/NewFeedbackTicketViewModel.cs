namespace TicketDesk.Web.Client.Models.Ticket
{
    using Domain.Model;
    using Infrastructure.Helpers;
    using System.ComponentModel.DataAnnotations;
    using Localization.Client;

    public class NewFeedbackTicketViewModel : TicketWithFilesBaseViewModel
    {
        private string[] _feedbackByValues;

        public Ticket Ticket { get; set; }

        public CustomFieldValue FeedbackBy { get; set; }

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

                _feedbackByValues = CustomFieldForHelper.GetMultilingualCustomFieldValuesAsArray(FeedbackBy.CustomField);
                return _feedbackByValues;
            }
        }

        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Views_Ticket_Feedback_FeedbackByRequired")]
        public string SelectedFeedbackByValue { get; set; }

        public NewFeedbackTicketViewModel()
        {
            Ticket = new Ticket();
            FeedbackBy = new CustomFieldValue();
            CustomerName = new CustomFieldValue();
            Feedback = new CustomFieldValue();
        }
    }
}