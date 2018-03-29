namespace TicketDesk.Web.Client.Infrastructure.Attribute
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using Localization.Client;

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class FileSizeAttribute : ValidationAttribute, IClientValidatable
    {
        public int MaxBytes { get; set; }

        private int MaxMegaByte => MaxBytes / 1048576;

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            IEnumerable<HttpPostedFileBase> files = value as IEnumerable<HttpPostedFileBase>;

            if (files != null && files.Any())
            {
                var totalBytes = files.Where(file => file != null).Sum(file => file.ContentLength);

                if (totalBytes > MaxBytes)
                {
                    return new ValidationResult(string.Format(Resources.FilesOversizeErrorMessage, MaxMegaByte));
                }
            }

            return ValidationResult.Success;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ValidationType = "filesize",
                ErrorMessage = string.Format(Resources.FilesOversizeErrorMessage, MaxMegaByte)
            };

            rule.ValidationParameters["maxbytes"] = MaxBytes;
            yield return rule;
        }
    }
}