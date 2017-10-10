namespace TicketDesk.Web.Client.Models.Ticket
{
    using System.Collections.Generic;
    using System.Linq;

    public class TicketCustomField
    {
        public int CategoryId { get; set; }

        public string CategoryName { get; set; }

        public int CustomFieldId { get; set; }

        public string CustomFieldName { get; set; }

        public string CustomFieldPossibleValue { get; set; }

        public int CustomFieldTypeId { get; set; }

        public string CustomFieldType { get; set; }

        public string FormName { get; set; }

        public string Value { get; set; }

        public IList<string> PossibleValues { get; set; }

        public IList<string> ConvertValuesToList(string values)
        {
            return values.Split(',').ToList();
        }
    }
}