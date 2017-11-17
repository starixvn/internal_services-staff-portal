namespace TicketDesk.Web.Client.Models.Ticket
{
    using Domain.Model;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Text.RegularExpressions;

    public class NewFacilityTicketViewModel : TicketWithFilesBaseViewModel
    {
        private string[] _requestTypes;

        public Ticket Ticket { get; set; }

        public CustomFieldValue RequestType { get; set; }

        public ICollection<Item> Items { get; set; }

        public string[] RequestTypes
        {
            get
            {
                if (_requestTypes != null)
                {
                    return _requestTypes;
                }

                _requestTypes = Regex.Split(RequestType.CustomField.Values, "\r\n");
                return _requestTypes;
            }
        }

        [Required]
        public string SelectedRequestType { get; set; }

        public NewFacilityTicketViewModel()
        {
            Ticket = new Ticket();
            RequestType = new CustomFieldValue();
            Items = new List<Item>();
        }
    }
    
    public class Item
    {
        public string Index { get; set; }

        public CustomFieldValue ItemName { get; set; }

        public CustomFieldValue Reason { get; set; }

        public Item()
        {
            ItemName = new CustomFieldValue();
            Reason = new CustomFieldValue();
        }
    }
}