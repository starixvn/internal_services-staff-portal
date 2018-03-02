namespace TicketDesk.Service.Interface
{
    using System.Collections.Generic;
    using Domain.Model;

    public interface IDataConfigurationService
    {
        IEnumerable<EditableField> GetEditableCustomFields();

        IEnumerable<CustomFieldListValue> GetEditableListValues(int customFieldId);

        bool AddCustomFieldListValue(CustomFieldListValue customFieldListValue);

        bool EditCustomFieldListValue(CustomFieldListValue modifiedCustomFieldListValue);
    }
}
