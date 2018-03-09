namespace TicketDesk.Web.Client.Models.Ticket
{
    using Domain.Model;
    using Infrastructure.Helpers;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Localization.Client;

    public class NewFacilityTicketViewModel : TicketWithFilesBaseViewModel
    {
        private string[] _requestTypes;

        public Ticket Ticket { get; set; }

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
    }
}