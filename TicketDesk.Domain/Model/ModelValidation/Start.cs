namespace TicketDesk.Domain.Model.ModelValidation
{
    using FluentValidation.Mvc;

    public static class Start
    {
        public static void Configure()
        {
            FluentValidationModelValidatorProvider.Configure();
        }
    }
}
