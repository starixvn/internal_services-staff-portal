namespace TicketDesk.Web.Client.Infrastructure.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Web.Mvc;
    using Antlr.Runtime.Misc;
    using Core.Enumerations;
    using Domain.Model;
    using Models.Ticket;

    public static class CustomFieldForHelper
    {
        //public static MvcHtmlString CustomFieldFor<TModel, TValue>(
        //    this HtmlHelper<TModel> html,
        //    Expression<Func<TModel, TValue>> propertyExpression)
        //{
        //    var htmlFieldName = ExpressionHelper.GetExpressionText(propertyExpression);
        //    return new MvcHtmlString("<p>TEST</p>");
        //}

        public static MvcHtmlString CustomFieldFor(
            this HtmlHelper html,
            CustomField value,
            object htmlAttributes = null)
        {
            var str = BuildHtmlString(value, htmlAttributes);

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
            object htmlAttributes = null)
        {
            TagBuilder input = null;
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
                    foreach (string s in values)
                    {
                        TagBuilder option = new TagBuilder("option");
                        option.Attributes.Add("value", s);
                        option.SetInnerText(s);
                        input.InnerHtml += option.ToString();
                    }
                    break;
                case (int)CustomFieldTypes.String:
                    input = new TagBuilder("input");
                    input.Attributes.Add("type", "text");
                    input.Attributes.Add("id", "customfield_" + customField.Id);
                    input.Attributes.Add("name", customField.Id.ToString());
                    input.Attributes.Add("placeholder", customField.Values);
                    break;
                case (int)CustomFieldTypes.TextArea:
                    input = new TagBuilder("textarea");
                    input.Attributes.Add("id", "customfield_" + customField.Id);
                    input.Attributes.Add("name", customField.Id.ToString());
                    input.Attributes.Add("placeholder", customField.Values);
                    break;
            }

            if (htmlAttributes != null)
            {
                var attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
                input?.MergeAttributes(attributes);
            }

            return input?.ToString();
        }

        public static MvcHtmlString EditorForCustomFieldItem(this HtmlHelper html, Item item)
        {
            var guid = Guid.NewGuid().ToString();

            // Index tag
            TagBuilder indexTag = new TagBuilder("input");
            indexTag.Attributes.Add("type", "hidden");
            indexTag.Attributes.Add("name", "Items.Index");
            indexTag.Attributes.Add("value", guid);

            var itemNameLabel = CustomFieldLabelFor(html, item.ItemName, new { For = "Items.[" + guid + "].ItemName" });
            var itemNameTag = CustomFieldFor(html, item.ItemName,
                new { Name = "Items.[" + guid + "].ItemName", Id = "Items.[" + guid + "].ItemName" });
            var reasonLabel = CustomFieldLabelFor(html, item.Reason, new { For = "Items.[" + guid + "].Reason" });
            var reasonTag = CustomFieldFor(html, item.Reason,
                new { Name = "Items.[" + guid + "].Reason", Id = "Items.[" + guid + "].Reason" });
            
            return new MvcHtmlString(indexTag + itemNameLabel.ToString() + itemNameTag + reasonLabel + reasonTag);
        }
    }
}