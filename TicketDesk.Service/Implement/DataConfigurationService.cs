namespace TicketDesk.Service.Implement
{
    using System.Collections.Generic;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using Domain.Model;
    using Interface;

    public class DataConfigurationService : EntityFrameworkBaseService, IDataConfigurationService
    {
        public IEnumerable<CustomField> GetEditableCustomFields()
        {
            var editableCustomFieldIds = new List<int> { 1, 5, 1021 };
            return context.CustomFields.Where(x => editableCustomFieldIds.Contains(x.Id));
        }

        public IEnumerable<CustomFieldListValue> GetEditableListValues(int customFieldId)
        {
            return context.CustomFieldListValues.Where(x => x.CustomFieldId == customFieldId);
        }

        public bool AddCustomFieldListValue(CustomFieldListValue customFieldListValue)
        {
            context.CustomFieldListValues.Add(customFieldListValue);
            var result = context.SaveChanges();
            return result > 0;
        }

        public bool EditCustomFieldListValue(CustomFieldListValue modifiedCustomFieldListValue)
        {
            context.CustomFieldListValues.AddOrUpdate(modifiedCustomFieldListValue);
            var result = context.SaveChanges();
            return result > 0;
        }
    }
}
