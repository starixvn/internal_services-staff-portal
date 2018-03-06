namespace TicketDesk.Web.Client.Infrastructure.Helpers
{
    using Core.Enumerations;
    using Domain.Model;
    using Localization.Client;
    using System;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Web.Mvc;

    public static class CustomFieldForHelper
    {
        private static TagBuilder BuildHiddenTag(
            string fieldName,
            string property,
            string valueAttribute)
        {
            TagBuilder hiddentTagBuilder = new TagBuilder("input")
            {
                Attributes =
                {
                    ["id"] = $"{fieldName}.{property}",
                    ["name"] = $"{fieldName}.{property}",
                    ["value"] = valueAttribute,
                    ["type"] = "hidden"
                }
            };

            return hiddentTagBuilder;
        }

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

        public static MvcHtmlString CustomFieldIndex(this HtmlHelper html, string parentName, string guid)
        {
            // Index tag
            TagBuilder indexTag = new TagBuilder("input");
            indexTag.Attributes.Add("type", "hidden");
            indexTag.Attributes.Add("name", $"{parentName}.Index");
            indexTag.Attributes.Add("value", guid);

            TagBuilder itemIndexTag = new TagBuilder("input");
            itemIndexTag.Attributes.Add("type", "hidden");
            itemIndexTag.Attributes.Add("name", $"{parentName}[{guid}].Index");
            itemIndexTag.Attributes.Add("value", guid);

            return new MvcHtmlString(indexTag.ToString() + itemIndexTag);
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

            string innerText = GetMultiligualStringOfCustomField(customFieldValue);

            if (customFieldValue != null && customFieldValue.CustomField.IsRequired)
            {
                label.InnerHtml = innerText + "<span class=\"highlight\">&nbsp;*</span>";
            }
            else
            {
                label.SetInnerText(innerText);
            }

            if (htmlAttributes != null)
            {
                var attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
                label.MergeAttributes(attributes);
            }

            return new MvcHtmlString(label.ToString());
        }

        private static string GetMultiligualStringOfCustomField(CustomFieldValue customFieldValue)
        {
            string languageSuffix = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;
            if (languageSuffix != "en")
            {
                PropertyInfo propertyInfo = typeof(CustomField).GetProperty("Name_" + languageSuffix);
                return propertyInfo?.GetValue(customFieldValue?.CustomField, null).ToString();
            }

            return customFieldValue?.CustomField.Name;
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
                string innerText = GetMultiligualStringOfCustomField(customFieldValue);

                if (customFieldValue.CustomField.IsRequired)
                {
                    label.InnerHtml = innerText + "<span class=\"highlight\">&nbsp;*</span>";
                }
                else
                {
                    label.SetInnerText(innerText);
                }
            }

            if (htmlAttributes != null)
            {
                var attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
                label.MergeAttributes(attributes);
            }

            return new MvcHtmlString(label.ToString());
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

            TagBuilder id = BuildHiddenTag(
                fieldName,
                nameof(customFieldValue.CustomFieldId),
                customFieldValue.CustomField.Id.ToString());

            if (!string.IsNullOrEmpty(parentName) && !string.IsNullOrEmpty(guid))
            {
                input = TransformTag(input, fieldName, nameof(customFieldValue.Value), parentName, guid);
                id = TransformTag(id, fieldName, nameof(customFieldValue.CustomFieldId), parentName, guid);
            }

            if (htmlAttributes != null)
            {
                var attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
                input.MergeAttributes(attributes);
            }

            return id.ToString() + input;
        }

        private static TagBuilder BuildInputTag(CustomFieldValue customFieldValue, string fieldName)
        {
            TagBuilder input;
            ////string idString = fieldName + ".Value";
            ////string nameString = fieldName + "_Value";
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
                    string[] values = GetMultilingualCustomFieldValuesAsArray(customFieldValue.CustomField);
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
                    input.Attributes.Add("placeholder", GetMultilingualCustomFieldValuesAsString(customFieldValue.CustomField));
                    break;
                //// case (int)CustomFieldTypes.String:
                default:
                    input = new TagBuilder("input");
                    input.Attributes.Add("type", "text");
                    input.Attributes.Add("id", fieldName + ".Value");
                    input.Attributes.Add("name", fieldName + ".Value");
                    input.Attributes.Add("placeholder", GetMultilingualCustomFieldValuesAsString(customFieldValue.CustomField));
                    break;
            }

            AddUnobtrusiveValidations(customFieldValue, input);

            return input;
        }

        private static void AddUnobtrusiveValidations(CustomFieldValue customFieldValue, TagBuilder input)
        {
            RequiredUnobtrusiveValidation(customFieldValue, input);

            if ((input.Attributes.ContainsKey("type") && input.Attributes["type"] == "text")
                || input.TagName == "textarea")
            {
                RegexPatternUnobtrusiveValidation(customFieldValue, input);

                TextLenghtUnobtrusiveValidation(customFieldValue, input);
            }
        }

        private static void RegexPatternUnobtrusiveValidation(CustomFieldValue customFieldValue, TagBuilder input)
        {
            if (!string.IsNullOrEmpty(customFieldValue.CustomField.Regex))
            {
                AddAttributes(input, "data-val", "true");
                input.Attributes.Add("data-val-regex", customFieldValue.CustomField.RegexValidationErrorMessage);
                input.Attributes.Add("data-val-regex-pattern", customFieldValue.CustomField.Regex);
            }
        }

        private static void TextLenghtUnobtrusiveValidation(CustomFieldValue customFieldValue, TagBuilder input)
        {
            int max = customFieldValue.CustomField.MaxLength;
            int min = customFieldValue.CustomField.MinLength;

            if (min != -1)
            {
                AddAttributes(input, "data-val", "true");
                input.Attributes.Add("data-val-length-min", min.ToString());
            }

            if (max != -1)
            {
                AddAttributes(input, "data-val", "true");
                input.Attributes.Add("data-val-length-max", max.ToString());
            }

            if (min != -1 && max != -1)
            {
                AddAttributes(input, "data-val-length",
                    string.Format(
                        Resources.CustomFieldForHelper_AddUnobtrusiveValidations_LengthRangeWarning,
                        customFieldValue.CustomField.Name,
                        min,
                        max));
            }

            if (min == -1 && max != -1)
            {
                AddAttributes(input, "data-val-length",
                    string.Format(
                        Resources.CustomFieldForHelper_AddUnobtrusiveValidations_ShorterLengthWarning,
                        customFieldValue.CustomField.Name,
                        max));
            }

            if (min != -1 && max == -1)
            {
                AddAttributes(input, "data-val-length",
                    string.Format(
                        Resources.CustomFieldForHelper_AddUnobtrusiveValidations_LongerLengthWarning,
                        customFieldValue.CustomField.Name,
                        min));
            }
        }

        private static void RequiredUnobtrusiveValidation(CustomFieldValue customFieldValue, TagBuilder input)
        {
            if (customFieldValue.CustomField.IsRequired)
            {
                AddAttributes(input, "data-val", "true");
                input.Attributes.Add("data-val-required", customFieldValue.CustomField.Name + Resources.CustomFieldForHelper_AddUnobtrusiveValidations_IsRequired);
            }
        }

        private static void AddAttributes(TagBuilder tag, string key, string value)
        {
            if (!tag.Attributes.ContainsKey(key))
            {
                tag.Attributes.Add(key, value);
            }
        }

        public static string[] GetMultilingualCustomFieldValuesAsArray(CustomField customField)
        {
            string valueString = GetMultilingualCustomFieldValuesAsString(customField);

            return valueString != null ? Regex.Split(valueString, "\r\n") : null;
        }

        private static string GetMultilingualCustomFieldValuesAsString(CustomField customField)
        {
            string languageSuffix = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;
            if (languageSuffix != "en")
            {
                PropertyInfo propertyInfo = typeof(CustomField).GetProperty("Values_" + languageSuffix);
                return propertyInfo?.GetValue(customField, null)?.ToString();
            }

            return customField.Values;
        }

        /// <summary>
        /// Tranform a normal tag to an guid index with parent property tag for correct binding to a list of object
        /// </summary>
        /// <param name="originalTag"></param>
        /// <param name="fieldName"></param>
        /// <param name="property"></param>
        /// <param name="parentName"></param>
        /// <param name="guid"></param>
        /// <returns></returns>
        private static TagBuilder TransformTag(TagBuilder originalTag, string fieldName, string property, string parentName, string guid)
        {
            originalTag.Attributes["id"] = $"{parentName}[{guid}]_{fieldName}_{property}";
            originalTag.Attributes["name"] = $"{parentName}[{guid}].{fieldName}.{property}";

            return originalTag;
        }
    }
}