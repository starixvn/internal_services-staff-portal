namespace TicketDesk.Web.Client.Infrastructure.Helpers
{
    using System;
    using System.Reflection;
    using System.Web.Mvc;
    using System.Web.Mvc.Html;
    using Attribute;
    using Core.Enumerations;

    public static class RazorViewHelper
    {
        public static MvcHtmlString PartialWithPrefix<TModel, TProperty>(this HtmlHelper<TModel> helper, System.Linq.Expressions.Expression<Func<TModel, TProperty>> expression, string partialViewName)
        {
            // *Note: This will not work with nested partial render
            string name = ExpressionHelper.GetExpressionText(expression);
            object model = ModelMetadata.FromLambdaExpression(expression, helper.ViewData).Model;
            var viewData = new ViewDataDictionary(helper.ViewData)
            {
                TemplateInfo = new TemplateInfo
                {
                    HtmlFieldPrefix = name
                }
            };

            return helper.Partial(partialViewName, model, viewData);
        }

        public static string GetFileTypeAttribute(Type t)
        {
            PropertyInfo propertyInfo = t.GetProperty("Files");
            if (propertyInfo != null)
            {
                FileTypeAttribute myAttribute = (FileTypeAttribute)Attribute.GetCustomAttribute(propertyInfo, typeof(FileTypeAttribute));

                return string.Join(", ", myAttribute.ValidTypes);
            }

            return string.Empty;
        }

        public static int GetNumberOfFileAttribute(Type t)
        {
            PropertyInfo propertyInfo = t.GetProperty("Files");
            if (propertyInfo != null)
            {
                NumberOfFileAttribute myAttribute = (NumberOfFileAttribute)Attribute.GetCustomAttribute(propertyInfo, typeof(NumberOfFileAttribute));

                return myAttribute.Max;
            }

            return -1;
        }

        public static int GetFileSizeAttribute(Type t, SizeUnit unit)
        {
            PropertyInfo propertyInfo = t.GetProperty("Files");
            if (propertyInfo != null)
            {
                FileSizeAttribute myAttribute = (FileSizeAttribute)Attribute.GetCustomAttribute(propertyInfo, typeof(FileSizeAttribute));

                return unit == SizeUnit.Megabyte 
                    ? myAttribute.MaxBytes / 1048576 
                    : myAttribute.MaxBytes;
            }

            return -1;
        }
    }
}