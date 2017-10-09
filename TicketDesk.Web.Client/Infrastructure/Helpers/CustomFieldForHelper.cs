namespace TicketDesk.Web.Client.Infrastructure.Helpers
{
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Web.Mvc;
    using Antlr.Runtime.Misc;
    using Domain.Model;

    public static class CustomFieldForHelper
    {
        public static MvcHtmlString CustomFieldFor<TModel, TValue>(
            this HtmlHelper<TModel> html,
            Expression<Func<TModel, TValue>> propertyExpression)
        {
            var htmlFieldName = ExpressionHelper.GetExpressionText(propertyExpression);
            return new MvcHtmlString("<p>TEST</p>");
        }

        public static MvcHtmlString CustomFieldFor(
            this HtmlHelper html,
            CustomFieldValue value)
        {
            return new MvcHtmlString("<p>TEST</p>");
        }
    }
}