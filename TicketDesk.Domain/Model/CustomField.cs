namespace TicketDesk.Domain.Model
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class CustomField
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public string Name_vi { get; set; }

        public int CustomFieldTypeId { get; set; }

        public string Values { get; set; }

        public string Values_vi { get; set; }

        public bool IsEnabled { get; set; }

        public int DisplayOrder { get; set; }

        public bool IsRequired { get; set; }

        public string Regex { get; set; }

        public string RegexValidationErrorMessage { get; set; }

        public int MinLength { get; set; }

        public int MaxLength { get; set; }

        [NotMapped]
        public CustomFieldType CustomFieldType { get; set; }

        [NotMapped]
        public ICollection<TicketCategory> TicketCategories { get; set; }

        [NotMapped]
        public ICollection<CustomFieldValue> CustomFieldValues { get; set; }

    }
}