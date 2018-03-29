namespace TicketDesk.Web.Client.Infrastructure.Attribute
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using Localization.Client;

    [AttributeUsage(AttributeTargets.Property)]
    public class NumberOfFileAttribute : ValidationAttribute, IClientValidatable
    {
        private readonly string ErrorMessageWithFormat = Resources.MaximumNumberOfFileErrorMessage;

        public int Max { get; set; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            IEnumerable<HttpPostedFileBase> files = value as IEnumerable<HttpPostedFileBase>;

            return files != null && files.Count() > Max
                ? new ValidationResult(string.Format(ErrorMessageWithFormat, Max))
                : ValidationResult.Success;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(
            ModelMetadata metadata,
            ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ValidationType = "numberoffile",
                ErrorMessage = string.Format(ErrorMessageWithFormat, Max)
            };

            rule.ValidationParameters.Add("max", Max);
            yield return rule;
        }
    }
}