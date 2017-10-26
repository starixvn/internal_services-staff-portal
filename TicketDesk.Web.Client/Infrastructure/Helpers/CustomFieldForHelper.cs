namespace TicketDesk.Web.Client.Infrastructure.Helpers
{
    using Core.Enumerations;
    using Domain.Model;
    using System;
    using System.Linq.Expressions;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Web.Mvc;

    public static class CustomFieldForHelper
    {
        public static MvcHtmlString CustomFieldFor<TModel, TValue>(
            this HtmlHelper<TModel> html,
            Expression<Func<TModel, TValue>> propertyExpression,
            object htmlAttributes = null) where TModel
            : class
        {
            var item = propertyExpression.Compile()(html.ViewData.Model) as CustomFieldValue;
            var htmlFieldName = ExpressionHelper.GetExpressionText(propertyExpression);

            var str = BuildHtmlString(item, fieldName: htmlFieldName, htmlAttributes: htmlAttributes);

            return new MvcHtmlString(str);
        }

        public static MvcHtmlString CustomFieldWithGuidIndexFor<TModel, TValue>(
            this HtmlHelper<TModel> html,
            Expression<Func<TModel, TValue>> propertyExpression,
            string guid,
            string parentName,
            object htmlAttributes = null)
            where TModel
            : class
        {
            var item = propertyExpression.Compile()(html.ViewData.Model) as CustomFieldValue;
            var htmlFieldName = ExpressionHelper.GetExpressionText(propertyExpression);

            var str = BuildHtmlString(item, guid, parentName, htmlFieldName, htmlAttributes);

            return new MvcHtmlString(str);
        }

        public static MvcHtmlString CustomFieldIndex(this HtmlHelper html, string parentName, string guid)
        {
            // Index tag
            TagBuilder indexTag = new TagBuilder("input");
            indexTag.Attributes.Add("type", "hidden");
            indexTag.Attributes.Add("name", parentName + ".Index");
            indexTag.Attributes.Add("value", guid);

            TagBuilder itemIndexTag = new TagBuilder("input");
            itemIndexTag.Attributes.Add("type", "hidden");
            itemIndexTag.Attributes.Add("name", parentName + "[" + guid + "].Index");
            itemIndexTag.Attributes.Add("value", guid);

            return new MvcHtmlString(indexTag.ToString() + itemIndexTag);
        }

        public static MvcHtmlString CustomFieldLabelWithGuidIndexFor(
            this HtmlHelper html,
            CustomField customField,
            string guid,
            string parentName,
            object htmlAttributes = null)
        {
            TagBuilder label = new TagBuilder("label");
            label.Attributes.Add("for", parentName + "[" + guid + "].customfield_" + customField.Id);
            label.SetInnerText(customField.Name);

            if (htmlAttributes != null)
            {
                var attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
                label.MergeAttributes(attributes);
            }

            return new MvcHtmlString(label.ToString());
        }

        public static MvcHtmlString CustomFieldLabelWithGuidIndexFor<TModel, TValue>(
            this HtmlHelper<TModel> html,
            Expression<Func<TModel, TValue>> propertyExpression,
            string guid,
            string parentName,
            object htmlAttributes = null)
        {
            var customFieldValue = propertyExpression.Compile()(html.ViewData.Model) as CustomFieldValue;
            var htmlFieldName = ExpressionHelper.GetExpressionText(propertyExpression);

            TagBuilder label = new TagBuilder("label");
            label.Attributes.Add("for", parentName + "[" + guid + "]." + htmlFieldName);
            if (customFieldValue != null)
            {
                label.SetInnerText(customFieldValue.CustomField.Name);
            }

            if (htmlAttributes != null)
            {
                var attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
                label.MergeAttributes(attributes);
            }

            return new MvcHtmlString(label.ToString());
        }

        public static MvcHtmlString CustomFieldLabelFor<TModel, TValue>(
            this HtmlHelper<TModel> html,
            Expression<Func<TModel, TValue>> propertyExpression,
            object htmlAttributes = null)
        {
            var customFieldValue = propertyExpression.Compile()(html.ViewData.Model) as CustomFieldValue;
            var htmlFieldName = ExpressionHelper.GetExpressionText(propertyExpression);

            TagBuilder label = new TagBuilder("label");
            label.Attributes.Add("for", htmlFieldName + ".Value");
            label.SetInnerText(customFieldValue?.CustomField.Name);

            if (htmlAttributes != null)
            {
                var attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
                label.MergeAttributes(attributes);
            }

            return new MvcHtmlString(label.ToString());
        }

        private static string BuildHtmlString(
            CustomFieldValue customFieldValue,
            string guid = null,
            string parentName = null,
            string fieldName = null,
            object htmlAttributes = null)
        {
            if (customFieldValue == null)
            {
                return null;
            }

            var input = BuildInputTag(customFieldValue, fieldName);

            if (htmlAttributes != null)
            {
                var attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
                input.MergeAttributes(attributes);
            }

            TagBuilder customFieldIdTagBuilder = new TagBuilder("input")
            {
                Attributes =
                {
                    ["id"] = parentName + "[" + guid + "]." + fieldName + ".CustomFieldId",
                    ["name"] = parentName + "[" + guid + "]." + fieldName + ".CustomFieldId",
                    ["value"] = customFieldValue.CustomField.Id.ToString(),
                    ["type"] = "hidden"
                }
            };

            if (!string.IsNullOrEmpty(guid) && !string.IsNullOrEmpty(parentName))
            {
                input.Attributes["id"] = parentName + "[" + guid + "]." + fieldName + ".Value";
                input.Attributes["name"] = parentName + "[" + guid + "]." + fieldName + ".Value";
            }
            else
            {
                customFieldIdTagBuilder.Attributes["id"] = fieldName + ".CustomFieldId";
                customFieldIdTagBuilder.Attributes["name"] = fieldName + ".CustomFieldId";
                customFieldIdTagBuilder.Attributes["value"] = customFieldValue.CustomField.Id.ToString();
            }

            return customFieldIdTagBuilder + input.ToString();
        }

        private static TagBuilder BuildInputTag(CustomFieldValue customFieldValue, string fieldName)
        {
            TagBuilder input;
            switch (customFieldValue.CustomField.CustomFieldTypeId)
            {
                case (int)CustomFieldTypes.Bool:
                    input = new TagBuilder("input");
                    input.Attributes.Add("type", "checkbox");
                    input.Attributes.Add("id", fieldName + ".Value");
                    input.Attributes.Add("name", fieldName + ".Value");
                    break;
                case (int)CustomFieldTypes.DateTime:
                    input = new TagBuilder("input");
                    input.Attributes.Add("type", "datetime-local");
                    input.Attributes.Add("id", fieldName + ".Value");
                    input.Attributes.Add("name", fieldName + ".Value");
                    break;
                case (int)CustomFieldTypes.Int:
                    input = new TagBuilder("input");
                    input.Attributes.Add("type", "number");
                    input.Attributes.Add("id", fieldName + ".Value");
                    input.Attributes.Add("name", fieldName + ".Value");
                    break;
                case (int)CustomFieldTypes.List:
                    var values = Regex.Split(customFieldValue.CustomField.Values, "\r\n");
                    input = new TagBuilder("select");
                    input.Attributes.Add("id", fieldName + ".Value");
                    input.Attributes.Add("name", fieldName + ".Value");
                    StringBuilder stringBuilder = new StringBuilder();
                    foreach (string s in values)
                    {
                        TagBuilder option = new TagBuilder("option");
                        option.Attributes.Add("value", s);
                        option.SetInnerText(s);
                        stringBuilder.Append(option);
                    }
                    input.InnerHtml = stringBuilder.ToString();
                    break;
                case (int)CustomFieldTypes.TextArea:
                    input = new TagBuilder("textarea");
                    input.Attributes.Add("id", fieldName + ".Value");
                    input.Attributes.Add("name", fieldName + ".Value");
                    input.Attributes.Add("placeholder", customFieldValue.CustomField.Values);
                    break;
                //// case (int)CustomFieldTypes.String:
                default:
                    input = new TagBuilder("input");
                    input.Attributes.Add("type", "text");
                    input.Attributes.Add("id", fieldName + ".Value");
                    input.Attributes.Add("name", fieldName + ".Value");
                    input.Attributes.Add("placeholder", customFieldValue.CustomField.Values);
                    break;
            }

            return input;
        }
    }
}