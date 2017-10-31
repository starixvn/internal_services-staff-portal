namespace TicketDesk.Web.Client.Infrastructure.Attribute
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;

    [AttributeUsage(AttributeTargets.Field|AttributeTargets.Property)]
    public class FileSizeAttribute : ValidationAttribute, IClientValidatable
    {
        private const string ErrorMessageWithFormat = "Total size of files must not exceed {0}";

        public int MaxBytes { get; set; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            IEnumerable<HttpPostedFileBase> files = value as IEnumerable<HttpPostedFileBase>;

            if (files != null)
            {
                var totalBytes = files.Sum(file => file.ContentLength);

                if (totalBytes > MaxBytes)
                {
                    return new ValidationResult(string.Format(ErrorMessageWithFormat, MaxBytes));
                }
            }

            return ValidationResult.Success;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ValidationType = "filesize",
                ErrorMessage = FormatErrorMessage(metadata.DisplayName)
            };
            rule.ValidationParameters["maxbytes"] = MaxBytes;
            yield return rule;
        }
    }
}