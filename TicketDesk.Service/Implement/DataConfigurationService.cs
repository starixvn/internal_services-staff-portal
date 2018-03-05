namespace TicketDesk.Service.Implement
{
    using System.Collections.Generic;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using Domain.Model;
    using Interface;

    public class DataConfigurationService : EntityFrameworkBaseService, IDataConfigurationService
    {
        public IEnumerable<EditableField> GetEditableCustomFields()
        {
            var result = new List<EditableField>();
            var editableCustomFieldIds = new List<int> { 1, 5, 1021 };

            foreach (var id in editableCustomFieldIds)
            {
                var categoryId = context.TicketCategoryCustomField
                    .Where(x => x.CustomFieldId == id)
                    .Select(x => x.CategoryId)
                    .First();
                var categoryName = context.TicketCategories
                    .Where(x => x.CategoryId == categoryId)
                    .Select(x => x.Name)
                    .First();
                var customFieldName = context.CustomFields
                    .Where(x => x.Id == id)
                    .Select(x => x.Name)
                    .First();
                var editTableField = new EditableField
                {
                    CustomFieldId = id,
                    Name = $"{categoryName} - {customFieldName}"
                };

                result.Add(editTableField);
            }

            return result;
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

        public bool IsCustomFieldDisplayNameViUnique(int customFieldId, string displayNameVi)
        {
            return !context.CustomFieldListValues.Any(x => x.CustomFieldId == customFieldId && x.DisplayName_vi == displayNameVi);
        }

        public bool IsCustomFieldDisplayNameEnUnique(int customFieldId, string displayNameEn)
        {
            return !context.CustomFieldListValues.Any(x => x.CustomFieldId == customFieldId && x.DisplayName_vi == displayNameEn);
        }
    }
}