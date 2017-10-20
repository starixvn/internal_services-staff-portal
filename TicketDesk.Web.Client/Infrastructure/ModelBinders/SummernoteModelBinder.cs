namespace TicketDesk.Web.Client
{
    using System.Web.Mvc;

    /// <summary>
    /// Cleans up after summernote's tendency to put a bunch of empty paragraph, break, and non-breaking spaces into an otherwise empty text field
    /// </summary>
    public class SummernoteModelBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            string comment = null;
            var val = GetValueFromValueProvider(bindingContext, false);
            if (val != null)
            {
                comment = val.AttemptedValue;
                comment = comment.StripHtmlWhenEmpty();
            }

            return comment;
        }

        public static ValueProviderResult GetValueFromValueProvider(
            ModelBindingContext bindingContext,
            bool performRequestValidation)
        {
            var unvalidatedValueProvider = bindingContext.ValueProvider as IUnvalidatedValueProvider;
            return (unvalidatedValueProvider != null)
                ? unvalidatedValueProvider.GetValue(bindingContext.ModelName, !performRequestValidation)
                : bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
        }
    }
}