namespace TicketDesk.Service.Implement
{
    using System.Collections.Generic;
    using System.Linq;
    using Core.Enumerations;
    using Domain.Model;
    using Interface;

    public class CustomFieldService : EntityFrameworkBaseService, ICustomFieldService
    {
        public List<CustomFieldValue> GetCustomFieldValuesForTicket(Ticket ticket)
        {
            var customFieldValues = context.CustomFieldValues.Where(x => x.TicketId == ticket.TicketId).ToList();
            var customFieldIds = customFieldValues.Select(x => x.CustomFieldId);
            var customFields = context.CustomFields.Where(x => customFieldIds.Contains(x.Id));

            foreach (CustomFieldValue customFieldValue in customFieldValues)
            {
                customFieldValue.CustomField = customFields.First(x => x.Id == customFieldValue.CustomFieldId);
            }

            return customFieldValues;
        }

        public IQueryable<CustomField> GetCustomFieldsForTicketCategory(TicketCategoryTypes type)
        {
            var customFieldIds = context.TicketCategoryCustomField
                .Where(x => x.CategoryId == (int)type)
                .Select(x => x.CustomFieldId);

            return context.CustomFields.Where(x => customFieldIds.Contains(x.Id));
        }

        public IQueryable<CustomField> GetCustomFieldsWithTypeForTicketCategory(TicketCategoryTypes type)
        {
            var customFieldIds = context.TicketCategoryCustomField
                .Where(x => x.CategoryId == (int)type)
                .Select(x => x.CustomFieldId);

            var availableCustomField = context.CustomFields.Where(x => customFieldIds.Contains(x.Id));

            foreach (var customField in availableCustomField)
            {
                customField.CustomFieldType = context.CustomFieldTypes.Find(customField.CustomFieldTypeId);
            }

            return availableCustomField;
        }

        public CustomFieldValue AddCustomFieldValue(string groupId, int customFieldId, string value, int ticketId)
        {
            var customFieldValue = new CustomFieldValue
            {
                GroupId = groupId,
                CustomFieldId = customFieldId,
                Value = value,
                TicketId = ticketId
            };

            return context.CustomFieldValues.Add(customFieldValue);
        }

        public bool AddCustomFieldValues(IEnumerable<CustomFieldValue> customFieldValues)
        {
            context.CustomFieldValues.AddRange(customFieldValues);
            var result = context.SaveChanges();

            return result > 0;
        }
    }
}
