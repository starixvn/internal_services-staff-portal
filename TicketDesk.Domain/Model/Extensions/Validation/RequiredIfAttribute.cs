namespace TicketDesk.Domain.Model.Extensions.Validation
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Web.Mvc;

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class RequiredIfAttribute : ValidationAttribute, IClientValidatable
    {
        private readonly RequiredAttribute _innerAttribute;

        public RequiredIfAttribute(string propertyName, object desiredvalue)
        {
            PropertyName = propertyName;
            DesiredValue = desiredvalue;
            _innerAttribute = new RequiredAttribute();
        }

        private object DesiredValue { get; }

        private string PropertyName { get; }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata,
            ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = ErrorMessageString,
                ValidationType = "requiredif",
            };

            rule.ValidationParameters["dependentproperty"] = PropertyName;
            rule.ValidationParameters["desiredvalue"] = DesiredValue is bool
                ? DesiredValue.ToString().ToLower(CultureInfo.InvariantCulture)
                : DesiredValue;

            yield return rule;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var dependentValue = validationContext.ObjectInstance.GetType().GetProperty(PropertyName)
                ?.GetValue(validationContext.ObjectInstance, null);

            if (dependentValue != null && dependentValue.ToString() == DesiredValue.ToString() &&
                !_innerAttribute.IsValid(value))
            {
                return new ValidationResult(
                    FormatErrorMessage(validationContext.DisplayName),
                    new[]
                    {
                        validationContext.MemberName
                    });
            }

            return ValidationResult.Success;
        }
    }
}