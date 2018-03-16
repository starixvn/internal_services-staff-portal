namespace TicketDesk.Service.Interface
{
    using System.Collections.Generic;
    using System.Linq;
    using Core.Enumerations;
    using Domain.Model;

    public interface ICustomFieldService
    {
        List<CustomFieldValue> GetCustomFieldValuesForTicket(Ticket ticket);

        IQueryable<CustomField> GetCustomFieldsForTicketCategory(TicketCategoryTypes type);

        IQueryable<CustomField> GetCustomFieldsWithTypeForTicketCategory(TicketCategoryTypes type);

        CustomFieldValue AddCustomFieldValue(string groupId, int customFieldId, string value, int ticketId);

        bool AddCustomFieldValues(IEnumerable<CustomFieldValue> customFieldValues);
    }
}
