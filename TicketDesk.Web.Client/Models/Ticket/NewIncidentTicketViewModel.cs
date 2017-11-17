namespace TicketDesk.Web.Client.Models.Ticket
{
    using System.Text.RegularExpressions;
    using Domain.Model;

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

                _reportByValues = Regex.Split(ReportBy.CustomField.Values, "\r\n");
                return _reportByValues;
            }
        }

        public CustomFieldValue TimeOfIncident { get; set; }

        public CustomFieldValue Desciption { get; set; }

        public NewIncidentTicketViewModel()
        {
            Ticket = new Ticket();
            ReportBy = new CustomFieldValue();
            TimeOfIncident = new CustomFieldValue();
            Desciption = new CustomFieldValue();
        }
    }
}