namespace TicketDesk.Web.Client.Models.Ticket
{
    using System.ComponentModel.DataAnnotations;
    using Domain.Model;
    using Infrastructure.Helpers;

    public class NewIncidentTicketViewModel : TicketWithFilesBaseViewModel
    {
        private string[] _reportByValues;

        public Ticket Ticket { get; set; }

        public CustomFieldValue ReportBy { get; set; }

        public string[] ReportByValues
        {
            get
            {
                if (_reportByValues != null)
                {
                    return _reportByValues;
                }

                _reportByValues = CustomFieldForHelper.GetMultilingualCustomFieldValuesAsArray(ReportBy.CustomField);
                return _reportByValues;
            }
        }

        public CustomFieldValue TimeOfIncident { get; set; }

        public CustomFieldValue Description { get; set; }

        [Required]
        public string SelectedReportByValue { get; set; }

        public NewIncidentTicketViewModel()
        {
            Ticket = new Ticket();
            ReportBy = new CustomFieldValue();
            TimeOfIncident = new CustomFieldValue();
            Description = new CustomFieldValue();
        }
    }
}