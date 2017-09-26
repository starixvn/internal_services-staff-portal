namespace TicketDesk.Web.Client
{
    using System.Linq;
    using System.Text;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Mvc.Ajax;
    using TicketDesk.Domain.Model;
    using TicketDesk.Localization.Infrastructure;

    public static class SortableColumnHeaderHelper
    {
        public static IHtmlString SortableColumnHeader(
            this AjaxHelper helper,
            HtmlHelper htmlHelper,
            UserTicketListSetting currentDisplayPreferences,
            int currentPage,
            string action,
            string listName,
            string sortColumn,
            string linkText,
            AjaxOptions ajaxOptions)
        {
            var imgContent = string.Empty;
            var linkContent = linkText;
            var indexContent = string.Empty;
            var sortColumns = currentDisplayPreferences.SortColumns;
            var column = sortColumns.SingleOrDefault(sc => sc.ColumnName == sortColumn);
            var arrowTop = "~/Content/Images/arrow_top.png";
            var arrowDown = "~/Content/Images/arrow_down.png";
            if (column != null)
            {
                var uh = new UrlHelper(helper.ViewContext.RequestContext);
                var imgSrc = uh.Content((column.SortDirection == ColumnSortDirection.Ascending) ? arrowTop : arrowDown);
                imgContent = string.Format("<img src='{0}' alt='{1}' />", imgSrc, column.SortDirection.ToString());

                var idx = sortColumns.IndexOf(column) + 1;
                indexContent = string.Format("<sup>[{0}]</sup>", idx);
            }

            var sb = new StringBuilder();
            sb.AppendLine("<span style='white-space:nowrap;'>");
            var line =
                // ReSharper disable once Mvc.ActionNotResolved
                helper.ActionLink(
                    linkContent,
                    "SortList",
                    new { page = currentPage, ListName = listName, ColumnName = sortColumn },
                    ajaxOptions,
                    new
                    {
                        OnMouseDown = "ticketCenter.sorts.setShiftStatus(event);",
                        Title = Strings.RemoveColumnsFromSort
                    }).ToString();

            sb.AppendLine(line + indexContent);
            sb.AppendLine(imgContent);
            sb.AppendLine("</span>");
            return MvcHtmlString.Create(sb.ToString());
        }
    }
}