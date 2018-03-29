namespace TicketDesk.Service.Interface
{
    using System.Collections.Generic;
    using Domain.Model;

    public interface IDataConfigurationService
    {
        IEnumerable<EditableField> GetEditableCustomFields();

        IEnumerable<CustomFieldListValue> GetEditableListValues(int customFieldId);

        CustomFieldListValue GetEditableListValue(int customFieldListValueId);

        bool CreateCustomFieldListValue(CustomFieldListValue customFieldListValue);

        bool UpdateCustomFieldListValue(CustomFieldListValue modifiedCustomFieldListValue);

        bool IsCustomFieldDisplayNameViUnique(int customFieldId, string displayNameVi, int customFieldListValueId);

        bool IsCustomFieldDisplayNameEnUnique(int customFieldId, string displayNameEn, int customFieldListValueId);
    }
}
