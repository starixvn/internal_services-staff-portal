namespace TicketDesk.Web.Client.Models.Settings
{
    using Localization;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Threading;
    using System.Web.Mvc;

    public class LanguageSettingsViewModel
    {
        public List<SelectListItem> SupportedLanguages
        {
            get
            {
                List<SelectListItem> items = new List<SelectListItem>();
                var languages = CultureHelper.Cultures;

                var currentCulture = Thread.CurrentThread.CurrentCulture;

                foreach (KeyValuePair<string, string> language in languages)
                {
                    SelectListItem item = new SelectListItem
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