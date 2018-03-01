namespace TicketDesk.Web.Client.Areas.Admin.Controllers
{
    using System.Web.Mvc;
    using Domain.Infrastructure;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Web.Attribute;

    [NapAuth(FunctionKey = FunctionKeys.DataConfiguration, Policy = Constant.Policy.Read)]
    public class DataConfigurationController : Controller
    {
        // GET: Admin/DataConfiguration
        public ActionResult Index()
        {

            return View();
        }
    }
}