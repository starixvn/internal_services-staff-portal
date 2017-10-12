namespace TicketDesk.Web.Client.Infrastructure.Helpers
{
    using System;
    using System.Web.Mvc;
    using System.Web.Mvc.Html;

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
    }
}