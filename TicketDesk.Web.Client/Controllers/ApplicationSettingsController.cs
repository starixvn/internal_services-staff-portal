// TicketDesk - Attribution notice
// Contributor(s):
//
//      Stephen Redd (https://github.com/stephenredd)
//
// This file is distributed under the terms of the Microsoft Public
// License (Ms-PL). See http://opensource.org/licenses/MS-PL
// for the complete terms of use.
//
// For any distribution that contains code from this file, this notice of
// attribution must remain intact, and a copy of the license must be
// provided to the recipient.

namespace TicketDesk.Web.Client.Controllers
{
    using Domain.Infrastructure;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Web.Attribute;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using TicketDesk.Domain;
    using TicketDesk.Domain.Model;
    using TicketDesk.Search.Azure;
    using TicketDesk.Search.Common;

    [RoutePrefix("admin/application-settings")]
    [Route("{action=index}")]
    public class ApplicationSettingsController : BaseController
    {
        public ApplicationSettingsController(TdDomainContext context)
        {
            Context = context;
            IsAzureSearchDetected = TdSearchContext.Current.IndexManager.GetType() == typeof(AzureIndexProvider);
        }

        private TdDomainContext Context { get; set; }

        private bool IsAzureSearchDetected { get; set; }

        [NapAuth(FunctionKey = FunctionKeys.ApplicationSettings, Policy = Constant.Policy.Read)]
        public ActionResult Index()
        {
            var databaseSetting = Context.TicketDeskSettings;
            ViewBag.IsAzureSearchEnabled = IsAzureSearchDetected;
            ViewBag.IsHost = NapContextHelper.HasRight("ApplicationSettings", Constant.Policy.Host);

            return View(databaseSetting);
        }

        [HttpPost]
        [NapAuth(FunctionKey = FunctionKeys.ApplicationSettings, Policy = Constant.Policy.Host)]
        public ActionResult Index(
            ApplicationSetting settings,
            [ModelBinder(typeof(CommaSeparatedModelBinder))] string[] categories,
            [ModelBinder(typeof(CommaSeparatedModelBinder))] string[] tickettypes,
            [ModelBinder(typeof(CommaSeparatedModelBinder))] string[] priorities,
            List<string> defaultroles,
            string defaultTextEditorType)
        {
            var databaseSetting = Context.TicketDeskSettings;
            if (ModelState.IsValid && TryUpdateModel(databaseSetting))
            {
                databaseSetting.SelectLists.CategoryList = categories.ToList();
                databaseSetting.SelectLists.PriorityList = priorities.ToList();
                databaseSetting.SelectLists.TicketTypesList = tickettypes.ToList();
                databaseSetting.SecuritySettings.DefaultNewUserRoles = defaultroles;
                databaseSetting.ClientSettings.Settings["DefaultTextEditorType"] = defaultTextEditorType;
                Context.SaveChanges();
            }

            ViewBag.IsAzureSearchEnabled = IsAzureSearchDetected;
            ViewBag.IsHost = NapContextHelper.HasRight(FunctionKeys.ApplicationSettings, Constant.Policy.Host);

            return View(databaseSetting);
        }
    }
}