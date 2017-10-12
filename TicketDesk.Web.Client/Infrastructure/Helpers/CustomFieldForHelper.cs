namespace TicketDesk.Web.Client.Infrastructure.Helpers
{
    using Core.Enumerations;
    using Domain.Model;
    using System;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Web.Mvc;

    public static class CustomFieldForHelper
    {
        public static MvcHtmlString CustomFieldFor(
            this HtmlHelper html,
            CustomField value,
            object htmlAttributes = null)
        {
            var str = BuildHtmlString(value, null, null, htmlAttributes);

            return new MvcHtmlString(str);
        }

        public static MvcHtmlString CustomFieldIndex(this HtmlHelper html, string parentName, string guid)
        {
            // Index tag
            TagBuilder indexTag = new TagBuilder("input");
            indexTag.Attributes.Add("type", "hidden");
            indexTag.Attributes.Add("name", parentName + ".Index");
            indexTag.Attributes.Add("value", guid);

            return new MvcHtmlString(indexTag.ToString());
        }

        public static MvcHtmlString CustomFieldLabelWithGuidIndexFor(
            this HtmlHelper html,
            CustomField customField,
            string guid,
            string parentName,
            object htmlAttributes = null)
        {
            TagBuilder label = new TagBuilder("label");
            label.Attributes.Add("for", parentName + ".[" + guid + "].customfield_" + customField.Id);
            label.SetInnerText(customField.Name);

            if (htmlAttributes != null)
            {
                var attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
                label.MergeAttributes(attributes);
            }

            return new MvcHtmlString(label.ToString());
        }

        public static MvcHtmlString CustomFieldWithGuidIndexFor(
            this HtmlHelper html,
            CustomField value,
            string guid,
            string parentName,
            object htmlAttributes = null)
        {
            
            var str = BuildHtmlString(value, Guid.NewGuid().ToString(), parentName, htmlAttributes);

            return new MvcHtmlString(str);
        }

        public static MvcHtmlString CustomFieldLabelFor(
            this HtmlHelper html,
            CustomField value,
            object htmlAttributes = null)
        {
            TagBuilder label = new TagBuilder("label");
            label.Attributes.Add("for", "customfield_" + value.Id);
            label.SetInnerText(value.Name);

            if (htmlAttributes != null)
            {
                var attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
                label.MergeAttributes(attributes);
            }

            return new MvcHtmlString(label.ToString());
        }

        private static string BuildHtmlString(
            CustomField customField,
            string guid = null,
            string parentName = null,
            object htmlAttributes = null)
        {
            TagBuilder input;
            switch (customField.CustomFieldTypeId)
            {
                case (int)CustomFieldTypes.Bool:
                    input = new TagBuilder("input");
                    input.Attributes.Add("type", "checkbox");
                    input.Attributes.Add("id", "customfield_" + customField.Id);
                    input.Attributes.Add("name", customField.Id.ToString());
                    break;
                case (int)CustomFieldTypes.DateTime:
                    input = new TagBuilder("input");
                    input.Attributes.Add("type", "datetime-local");
                    input.Attributes.Add("id", "customfield_" + customField.Id);
                    input.Attributes.Add("name", customField.Id.ToString());
                    break;
                case (int)CustomFieldTypes.Int:
                    input = new TagBuilder("input");
                    input.Attributes.Add("type", "number");
                    input.Attributes.Add("id", "customfield_" + customField.Id);
                    input.Attributes.Add("name", customField.Id.ToString());
                    break;
                case (int)CustomFieldTypes.List:
                    var values = Regex.Split(customField.Values, "\r\n");
                    input = new TagBuilder("select");
                    input.Attributes.Add("id", "customfield_" + customField.Id);
                    input.Attributes.Add("name", customField.Id.ToString());
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
                    input.Attributes.Add("id", "customfield_" + customField.Id);
                    input.Attributes.Add("name", customField.Id.ToString());
                    input.Attributes.Add("placeholder", customField.Values);
                    break;
                //// case (int)CustomFieldTypes.String:
                default:
                    input = new TagBuilder("input");
                    input.Attributes.Add("type", "text");
                    input.Attributes.Add("id", "customfield_" + customField.Id);
                    input.Attributes.Add("name", customField.Id.ToString());
                    input.Attributes.Add("placeholder", customField.Values);
                    break;
            }

            if (!string.IsNullOrEmpty(guid) && !string.IsNullOrEmpty(parentName))
            {
                input.Attributes["id"] = parentName + ".[" + guid + "].customfield_" + customField.Id;
                input.Attributes["name"] = parentName + ".[" + guid + "].customfield_" + customField.Id;
            }

            if (htmlAttributes != null)
            {
                var attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
                input.MergeAttributes(attributes);
            }

            return input.ToString();
        }
    }
}