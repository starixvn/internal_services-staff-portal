namespace TicketDesk.Web.Client.Infrastructure.Attribute
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;

    [AttributeUsage(AttributeTargets.Property)]
    public class FileTypeAttribute : ValidationAttribute, IClientValidatable
    {
        private readonly string _defaultErrorMessage = "Only the following file types are allowed: {0}";
        public IEnumerable<string> ValidTypes { get; }

        public FileTypeAttribute(string[] validTypes)
        {
            ValidTypes = validTypes.Select(x => x.ToLower(CultureInfo.InvariantCulture));
            ErrorMessage = string.Format(_defaultErrorMessage, string.Join(" or ", ValidTypes));
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            IEnumerable<HttpPostedFileBase> files = value as IEnumerable<HttpPostedFileBase>;
            if (files != null)
            {
                foreach (HttpPostedFileBase file in files)
                {
                    if (file != null && !ValidTypes.Any(e => file.FileName.EndsWith(e)))
                    {
                        return new ValidationResult(ErrorMessageString);
                    }
                }
            }
            return ValidationResult.Success;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ValidationType = "filetype",
                ErrorMessage = ErrorMessageString
            };
            rule.ValidationParameters.Add("validtypes", string.Join(",", ValidTypes));
            yield return rule;
        }
    }
}