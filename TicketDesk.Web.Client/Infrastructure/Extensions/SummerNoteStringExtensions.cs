namespace TicketDesk.Web.Client
{
    using System.Text.RegularExpressions;

    public static class SummerNoteStringExtensions
    {
        public static string StripHtmlWhenEmpty(this string content)
        {
            if (!string.IsNullOrEmpty(content))
            {
                var ctext = Regex.Replace(content, "<[^>]*>", string.Empty).ToLowerInvariant();
                var hasText = ctext.Replace("\n", " ").Replace("&nbsp;", " ").Trim().Length > 0;
                if (!hasText)
                {
                    return null;
                }
            }

            return content;
        }
    }
}