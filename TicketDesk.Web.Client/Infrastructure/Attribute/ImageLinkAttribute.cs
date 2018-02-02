namespace TicketDesk.Core.Model
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Net;
    using Resources;

    [AttributeUsage(AttributeTargets.Property)]
    public class ImageLinkAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                string imageUrl = value.ToString();
                bool validUrl = true;
                try
                {
                    var imageLink = new Uri(imageUrl);
                    var req = WebRequest.Create(imageLink);
                    WebResponse response = req.GetResponse();
                    validUrl = CheckValidImageUrlExtension(response, "jpeg") || CheckValidImageUrlExtension(response, "jpg") || CheckValidImageUrlExtension(response, "png");
                    return validUrl ? ValidationResult.Success : new ValidationResult(Messages.InvalidImageURL);
                }
                catch
                {
                    return new ValidationResult(Messages.InvalidImageURL);
                }
            }
            else
            {
                return new ValidationResult("" + validationContext.DisplayName + " is required");
            }
        }

        private static bool CheckValidImageUrlExtension(WebResponse response, string extension)
        {
            return response.ContentType.ToLower(CultureInfo.InvariantCulture)
                               .StartsWith(string.Format("image/{0}", extension));
        }
    }
}