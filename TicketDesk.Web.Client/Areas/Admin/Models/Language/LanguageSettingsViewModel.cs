namespace TicketDesk.Web.Client.Areas.Admin.Models.Language
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Threading;
    using System.Web.Mvc;
    using Localization;

    public class LanguageSettingsViewModel
    {
        public List<SelectListItem> SupportedLanguages
        {
            get
            {
                var items = new List<SelectListItem>();
                var languages = CultureHelper.Cultures;

                var currentCulture = Thread.CurrentThread.CurrentCulture;

                foreach (var language in languages)
                {
                    var item = new SelectListItem
                    {
                        Value = language.Key,
                        Text = language.Value
                    };

                    if (language.Key == currentCulture.IetfLanguageTag)
                    {
                        item.Selected = true;
                    }

                    items.Add(item);
                }

                return items;
            }
        }

        [Display(Name = "Select language")]
        public string SelectedLanguage { get; set; }
    }
}