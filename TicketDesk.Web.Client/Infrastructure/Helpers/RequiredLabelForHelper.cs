namespace TicketDesk.Web.Client.Infrastructure.Helpers
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Web.Mvc;

    public static class RequiredLabelForHelper
    {
        public static MvcHtmlString RequiredLabelFor<TModel, TValue>(
            this HtmlHelper<TModel> helper,
            Expression<Func<TModel, TValue>> expression,
            object htmlAttributes = null,
            bool isIncludeMaxlength = false)
        {
            var metaData = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);

            var htmlFieldName = ExpressionHelper.GetExpressionText(expression);
            var labelText = metaData.DisplayName ?? metaData.PropertyName ?? htmlFieldName.Split('.').Last();

            if (string.IsNullOrEmpty(labelText))
            {
                return MvcHtmlString.Empty;
            }

            labelText = BuildLabelInnerHtml(expression, isIncludeMaxlength, metaData);

            var label = new TagBuilder("label");
            label.Attributes.Add("for", helper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldId(htmlFieldName));

            if (htmlAttributes != null)
            {
                foreach (PropertyDescriptor prop in TypeDescriptor.GetProperties(htmlAttributes))
                {
                    label.MergeAttribute(prop.Name.Replace('_', '-'), prop.GetValue(htmlAttributes)?.ToString(), true);
                }
            }

            label.InnerHtml = labelText;
            return MvcHtmlString.Create(label.ToString());
        }

        private static string BuildLabelInnerHtml<TModel, TValue>(Expression<Func<TModel, TValue>> expression, bool isIncludeMaxlength,
            ModelMetadata metaData)
        {
            string labelText = "";
            if (metaData.IsRequired)
            {
                labelText += "<span class=\"highlight\">&nbsp;*</span>";
            }

            if (isIncludeMaxlength)
            {
                var member = expression.Body as MemberExpression;
                var propertyInfo = member?.Member as PropertyInfo;
                var maxLength = propertyInfo?.GetCustomAttribute<StringLengthAttribute>().MaximumLength;
                labelText += $"<span class=\"highlight\">({maxLength} characters)</span>";
            }

            return labelText;
        }
    }
}