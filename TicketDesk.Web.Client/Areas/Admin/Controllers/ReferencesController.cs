using System;
using System.Linq;
using System.Web.Mvc;
using TicketDesk.Core.Enumerations;
using TicketDesk.Domain;
using TicketDesk.Domain.Model;
using TicketDesk.Web.Client.Models.Admin;

namespace TicketDesk.Web.Client.Areas.Admin.Controllers
{
    public class ReferencesController : Controller
    {
        private readonly TdDomainContext Context = new TdDomainContext();

        public ActionResult TVSchedules()
        {
            var tvSchedulesViewModel = new ReferenceViewModel
            {
                References = Context.References.Where(x => x.ReferenceType == ReferenceType.TvSchedule).ToList()
            };

            return View(tvSchedulesViewModel);
        }

        [HttpPost]
        public ActionResult TVSchedules(ReferenceViewModel viewModel)
        {
            var references = viewModel.References;
            var userId = Context.SecurityProvider.CurrentUserId;
            if (references != null)
            {
                foreach (Reference reference in references)
                {
                    var tvSchedule = new Reference
                    {
                        DateAppear = reference.DateAppear,
                        ImageLink = reference.ImageLink,
                        ReferenceType = ReferenceType.TvSchedule,
                        LastUpdateBy = userId,
                        LastUpdateDate = DateTime.Now
                    };

                    Context.References.Add(tvSchedule);
                }
                Context.SaveChanges();
            }

            return Json(new JsonResult { });
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