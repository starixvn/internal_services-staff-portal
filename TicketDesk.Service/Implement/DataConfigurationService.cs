namespace TicketDesk.Service.Implement
{
    using System.Collections.Generic;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using System.Text;
    using Core.Enumerations;
    using Domain.Model;
    using Interface;
    using Microsoft.Practices.ObjectBuilder2;

    public class DataConfigurationService : EntityFrameworkBaseService, IDataConfigurationService
    {
        public IEnumerable<EditableField> GetEditableCustomFields()
        {
            var result = new List<EditableField>();

            var editableCustomFieldIds = context.CustomFields
                .Where(x => x.CustomFieldTypeId == (int)CustomFieldTypes.List && x.IsAllowedToEditListValue)
                .Select(x => x.Id);

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

            return result.OrderBy(x => x.Name);
        }

        public CustomFieldListValue GetEditableListValue(int customFieldListValueId)
        {
            return context.CustomFieldListValues.Find(customFieldListValueId);
        }

        public IEnumerable<CustomFieldListValue> GetEditableListValues(int customFieldId)
        {
            return context.CustomFieldListValues.Where(x => x.CustomFieldId == customFieldId).OrderBy(x => x.Id);
        }

        public bool CreateCustomFieldListValue(CustomFieldListValue customFieldListValue)
        {
            context.CustomFieldListValues.Add(customFieldListValue);
            var result = context.SaveChanges();
            UpdateCustomFieldValue(customFieldListValue.CustomFieldId);
            return result > 0;
        }

        public bool IsCustomFieldDisplayNameEnUnique(int customFieldId, string displayNameEn, int customFieldListValueId)
        {
            // customFieldListValueId = 0 => create new
            return customFieldListValueId == 0
                ? !context.CustomFieldListValues.Any(x =>
                    x.CustomFieldId == customFieldId && x.DisplayName == displayNameEn)
                : !context.CustomFieldListValues.Any(x =>
                    x.CustomFieldId == customFieldId && x.DisplayName == displayNameEn &&
                    x.Id != customFieldListValueId);
        }

        public bool IsCustomFieldDisplayNameViUnique(int customFieldId, string displayNameVi, int customFieldListValueId)
        {
            // customFieldListValueId = 0 => create new
            return customFieldListValueId == 0
                ? !context.CustomFieldListValues.Any(x =>
                    x.CustomFieldId == customFieldId && x.DisplayName_vi == displayNameVi)
                : !context.CustomFieldListValues.Any(x =>
                    x.CustomFieldId == customFieldId && x.DisplayName_vi == displayNameVi &&
                    x.Id != customFieldListValueId);
        }

        public bool UpdateCustomFieldListValue(CustomFieldListValue modifiedCustomFieldListValue)
        {
            context.CustomFieldListValues.AddOrUpdate(modifiedCustomFieldListValue);
            var result = context.SaveChanges();
            UpdateCustomFieldValue(modifiedCustomFieldListValue.CustomFieldId);
            return result >= 0;
        }

        private void UpdateCustomFieldValue(int customFieldId)
        {
            var customFieldActiveListValues =
                context.CustomFieldListValues.Where(x => x.CustomFieldId == customFieldId && x.IsActive).ToList();

            var customField = context.CustomFields.Find(customFieldId);

            if (customField != null)
            {
                StringBuilder displayNameEn = new StringBuilder();
                foreach (var customFieldActiveListValue in customFieldActiveListValues)
                {
                    displayNameEn.Append(string.IsNullOrEmpty(customFieldActiveListValue.DisplayName)
                        ? customFieldActiveListValue.DisplayName_vi
                        : customFieldActiveListValue.DisplayName);

                    displayNameEn.Append("\r\n");
                }

                customField.Values_vi = customFieldActiveListValues.Select(x => x.DisplayName_vi).JoinStrings("\r\n");
                customField.Values = displayNameEn.ToString().Trim();

                context.CustomFields.Attach(customField);
                var entry = context.Entry(customField);
                entry.Property(x => x.Values).IsModified = true;
                entry.Property(x => x.Values_vi).IsModified = true;

                context.SaveChanges();
            }
        }
    }
}