namespace TicketDesk.Web.Client.Models.Ticket
{
    using System.Collections.Generic;
    using Domain.Model;

    public interface ITicketCustomFieldsProvider
    {
        IList<CustomFieldValue> GetCustomFieldValuesToInsert(int ticketId);
    }
}
