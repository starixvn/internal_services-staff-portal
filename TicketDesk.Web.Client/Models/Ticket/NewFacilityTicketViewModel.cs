namespace TicketDesk.Web.Client.Models.Ticket
{
    using Domain.Model;
    using Infrastructure.Helpers;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Localization.Client;

    public class NewFacilityTicketViewModel : TicketWithFilesBaseViewModel, ITicketCustomFieldsProvider
    {
        private string[] _requestTypes;

        public Ticket Ticket { get; set; }

        public bool AllowToChooseRecipient { get; set; }

        public CustomFieldValue RequestType { get; set; }

        public ICollection<FacilityItem> Items { get; }

        public string[] RequestTypes
        {
            get
            {
                if (_requestTypes != null)
                {
                    return _requestTypes;
                }

                _requestTypes = CustomFieldForHelper.GetMultilingualCustomFieldValuesAsArray(RequestType.CustomField);
                return _requestTypes;
            }
        }

        [Required(ErrorMessageResourceName = nameof(Resources.RequiredErrorMessage), ErrorMessageResourceType = typeof(Resources))]
        [Display(Name = nameof(Resources.RequestType), ResourceType = typeof(Resources))]
        public string SelectedRequestType { get; set; }

        public NewFacilityTicketViewModel()
        {
            Ticket = new Ticket();
            RequestType = new CustomFieldValue();
            Items = new List<FacilityItem>();
        }

        public IList<CustomFieldValue> GetCustomFieldValuesToInsert(int ticketId)
        {
            var customFieldValues = new List<CustomFieldValue>();

            RequestType.GroupId = null;
            RequestType.TicketId = ticketId;
            customFieldValues.Add(RequestType);

            foreach (FacilityItem facilityItem in Items)
            {
                facilityItem.ItemName.GroupId = facilityItem.Index;
                facilityItem.ItemName.TicketId = ticketId;
                customFieldValues.Add(facilityItem.ItemName);

                facilityItem.Reason.GroupId = facilityItem.Index;
                facilityItem.Reason.TicketId = ticketId;
                customFieldValues.Add(facilityItem.Reason);
            }

            return customFieldValues;
        }
    }
}