namespace TicketDesk.Web.Client.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Web.Configuration;
    using System.Web.Mvc;
    using Domain.Infrastructure;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Web.Attribute;

    [RoutePrefix("References")]
    public class ReferencesController : BaseController
    {
        private readonly string _webRootPath;
        private readonly string _webPath;

        public ReferencesController()
        {
            _webPath = WebConfigurationManager.AppSettings["FileManager_RootPath"];
            _webRootPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _webPath);

            if (!Directory.Exists(_webRootPath))
            {
                Directory.CreateDirectory(_webRootPath);
            }
        }

        public ActionResult Index()
        {
            var items = GetIndexSubMenuItems();

            return View("_SubmenuNavigation", items);
        }

        [NapAuth(FunctionKey = FunctionKeys.FileManagement, Policy = Constant.Policy.Read)]
        [Route("bet-of-the-day")]
        public ActionResult BetOfTheDay()
        {
            ViewBag.FilePath = GetFilePath("BetOfTheDay");

            return View();
        }

        [NapAuth(FunctionKey = FunctionKeys.FileManagement, Policy = Constant.Policy.Read)]
        [Route("featured-event")]
        public ActionResult FeaturedEvent()
        {
            ViewBag.FilePath = GetFilePath("FeaturedEvent");

            return View();
        }

        [NapAuth(FunctionKey = FunctionKeys.FileManagement, Policy = Constant.Policy.Read)]
        [Route("newsletter")]
        public ActionResult Newsletter()
        {
            ViewBag.FilePath = GetFilePath("Newsletter");

            return View();
        }

        [NapAuth(FunctionKey = FunctionKeys.FileManagement, Policy = Constant.Policy.Read)]
        [Route("trading-limit")]
        public ActionResult TradingLimit()
        {
            ViewBag.FilePath = GetFilePath("TradingLimit");

            return View();
        }

        [NapAuth(FunctionKey = FunctionKeys.FileManagement, Policy = Constant.Policy.Read)]
        [Route("tv-schedules")]
        public ActionResult TvSchedules()
        {
            ViewBag.FilePath = GetFilePath("TvSchedules");

            return View();
        }

        private string GetFilePath(string folder)
        {
            var filePath = string.Empty;
            var today = DateTime.Now.Date;
            var directory = string.Format("{0}/{1}/{2}/{3}/{4}", _webRootPath, folder, today.Year, today.Month, today.Day);

            if (Directory.Exists(directory))
            {
                DirectoryInfo dirInfo = new DirectoryInfo(directory);
                var filters = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                IEnumerable<FileInfo> files = dirInfo.GetFiles().Where(f => filters.Contains(f.Extension, StringComparer.OrdinalIgnoreCase));

                if (files.Any())
                {
                    filePath = string.Format("~/{0}/{1}/{2}/{3}/{4}/{5}", _webPath, folder, today.Year, today.Month, today.Day, files.First().Name);
                }
            }

            return filePath;
        }
    }
}