using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TicketDesk.Web.Client.ViewModels.Ticket
{
    using TicketDesk.Domain.Model;

    public class CustomFieldViewModel : CustomField
    {
        public IList<string> PosibbleValues { get; set; }

        public IList<string> ConvertValuesToList(string values)
        {
            return values.Split('-').ToList();
        }

    }
}