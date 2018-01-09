namespace TicketDesk.Web.Client.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Domain;
    using Domain.Infrastructure;
    using Domain.Model;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Web.Attribute;
    using Search.Azure;
    using Search.Common;

    [RoutePrefix("admin/application-settings")]
    [Route("{action=index}")]
    public class ApplicationSettingsController : BaseController
    {
        public ApplicationSettingsController(TdDomainContext context)
        {
            Context = context;
            IsAzureSearchDetected = TdSearchContext.Current().IndexManager.GetType() == typeof(AzureIndexProvider);
        }

        private TdDomainContext Context { get; }

        private bool IsAzureSearchDetected { get; }

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