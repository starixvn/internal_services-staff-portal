namespace TicketDesk.Domain.Model
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using TicketDesk.Localization;

    public class TicketTag
    {
        public int TicketTagId { get; set; }

        [Required(ErrorMessageResourceName = "FieldRequired", ErrorMessageResourceType = typeof(Validation))]
        [StringLength(100, ErrorMessageResourceName = "FieldMaximumLength", ErrorMessageResourceType = typeof(Validation))]
        public string TagName { get; set; }

        public int TicketId { get; set; }

        public virtual Ticket Ticket { get; set; }

        #region utility

        public static string[] GetTagsFromString(string tagString)
        {
            var returnTags = new List<string>();
            if (!string.IsNullOrEmpty(tagString))
            {
                string[] tags = tagString.Split(',');
                foreach (string t in tags)
                {
                    var formattedTag = t.ToLowerInvariant().Trim();
                    if (!string.IsNullOrEmpty(formattedTag) && !returnTags.Contains(formattedTag))
                    {
                        returnTags.Add(formattedTag);
                    }
                }
            }

            return returnTags.ToArray();
        }

        #endregion utility
    }
}