namespace TicketDesk.Domain.Model.ModelValidation
{
    using FluentValidation;

    public class CustomFieldValueValidator : AbstractValidator<CustomFieldValue>
    {
        public CustomFieldValueValidator()
        {
            RuleFor(x => x.Value).NotNull()
                .When(x => x.CustomField.IsRequired)
                .WithMessage(x => $"{x.CustomFieldId} is required");
        }
    }
}