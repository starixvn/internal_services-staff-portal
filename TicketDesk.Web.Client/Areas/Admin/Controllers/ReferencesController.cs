using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using TicketDesk.Core.Enumerations;
using TicketDesk.Domain;

namespace TicketDesk.Web.Client.Areas.Admin.Controllers
{
    public class ReferencesController : Controller
    {
        private readonly string _webRootPath;
        private readonly string _webPath;
        private TdDomainContext Context = new TdDomainContext();

        public ReferencesController()
        {
            _webPath = WebConfigurationManager.AppSettings["FileManager_RootPath"];
            _webRootPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _webPath);

            if (!Directory.Exists(_webRootPath))
            {
                Directory.CreateDirectory(_webRootPath);
            }
        }

        public ActionResult TVSchedules()
        {
            var tvSchedulesList = Context.References.Where(x => x.ReferenceType == ReferenceType.TvSchedule).ToList();
            return View(tvSchedulesList);
        }

        // GET: Admin/References/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Admin/References/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/References/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Admin/References/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Admin/References/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Admin/References/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Admin/References/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
