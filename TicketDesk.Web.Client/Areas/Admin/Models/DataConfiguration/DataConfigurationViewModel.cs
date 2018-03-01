namespace TicketDesk.Web.Client.Areas.Admin.Models.DataConfiguration
{
    using System.Collections.Generic;

    public class DataConfigurationViewModel
    {
        public int SelectedFieldId { get; set; }

        public IList<string> SelectedFieldValues { get; set; }


    }
}