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
            var today = DateTime.Now.Date;
            var directory = string.Format("{0}/BetOfTheDay/{1}/{2}/{3}", _webRootPath, today.Year, today.Month, today.Day);
            ViewBag.FilePath = GetFilePath(directory);

            return View();
        }

        [NapAuth(FunctionKey = FunctionKeys.FileManagement, Policy = Constant.Policy.Read)]
        [Route("featured-event")]
        public ActionResult FeaturedEvent()
        {
            var today = DateTime.Now.Date;
            var directory = string.Format("{0}/FeaturedEvent/{1}/{2}/{3}", _webRootPath, today.Year, today.Month, today.Day);
            ViewBag.FilePath = GetFilePath(directory);

            return View();
        }

        [NapAuth(FunctionKey = FunctionKeys.FileManagement, Policy = Constant.Policy.Read)]
        [Route("newsletter")]
        public ActionResult Newsletter()
        {
            var today = DateTime.Now.Date;
            var directory = string.Format("{0}/Newsletter/{1}/{2}/{3}", _webRootPath, today.Year, today.Month, today.Day);
            ViewBag.FilePath = GetFilePath(directory);

            return View();
        }

        [NapAuth(FunctionKey = FunctionKeys.FileManagement, Policy = Constant.Policy.Read)]
        [Route("trading-limit")]
        public ActionResult TradingLimit()
        {
            var today = DateTime.Now.Date;
            var directory = string.Format("{0}/TradingLimit/{1}/{2}/{3}", _webRootPath, today.Year, today.Month, today.Day);
            ViewBag.FilePath = GetFilePath(directory);

            return View();
        }

        [NapAuth(FunctionKey = FunctionKeys.FileManagement, Policy = Constant.Policy.Read)]
        [Route("tv-schedules")]
        public ActionResult TvSchedules()
        {
            var today = DateTime.Now.Date;
            var directory = string.Format("{0}/TvSchedules/{1}/{2}/{3}", _webRootPath, today.Year, today.Month, today.Day);
            ViewBag.FilePath = GetFilePath(directory);

            return View();
        }

        private string GetFilePath(string directory)
        {
            var filePath = string.Empty;

            if (Directory.Exists(directory))
            {
                DirectoryInfo dirInfo = new DirectoryInfo(directory);
                var filters = new[] { ".jpg", ".jpeg", ".png" };
                IEnumerable<FileInfo> files = dirInfo.GetFiles().Where(f => filters.Contains(f.Extension, StringComparer.OrdinalIgnoreCase));

                if (files.Any())
                {
                    filePath = string.Format("~/{0}", Path.Combine(_webPath, files.First().Name));
                }
            }

            return filePath;
        }
    }
}