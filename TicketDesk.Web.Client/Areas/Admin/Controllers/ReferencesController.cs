namespace TicketDesk.Web.Client.Areas.Admin.Controllers
{
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Web.Attribute;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Web;
    using System.Web.Configuration;
    using System.Web.Mvc;
    using TicketDesk.Core.Enumerations;
    using TicketDesk.Domain;
    using TicketDesk.Domain.Infrastructure;
    using TicketDesk.Domain.Model;
    using TicketDesk.Web.Client.Models.Admin;

    public class ReferencesController : Controller
    {
        private readonly TdDomainContext Context = new TdDomainContext();
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

        public ActionResult TVSchedules()
        {
            var tvSchedulesViewModel = new ListReferencesViewModel
            {
                References = Context.References.Where(x => x.ReferenceType == ReferenceType.TvSchedule).ToList()
            };

            return View(tvSchedulesViewModel);
        }

        [HttpPost]
        [NapAuth(FunctionKey = FunctionKeys.TicketCenter, Policy = Constant.Policy.Read)]
        public ActionResult TVSchedules(ListReferencesViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var references = viewModel.References;

                if (references != null)
                {
                    RemoveOldReference(ReferenceType.TvSchedule);

                    foreach (Reference reference in references)
                    {
                        SaveEachReference(reference, ReferenceType.TvSchedule);
                    }
                    Context.SaveChanges();
                }
            }

            return Json(new JsonResult { });
        }

        public ActionResult ReferenceEntity(ReferenceType referenceType)
        {
            var reference = new Reference
            {
                ReferenceType = referenceType
            };
            return PartialView("_ReferenceEntryEditor", reference);
        }

        private void SaveEachReference(Reference reference, ReferenceType referenceType)
        {
            var tvSchedule = new Reference
            {
                DateAppear = reference.DateAppear,
                ImageLink = reference.ImageLink,
                ReferenceType = referenceType,
                LastUpdateBy = Context.SecurityProvider.CurrentUserId,
                LastUpdateDate = DateTime.Now
            };

            Context.References.Add(tvSchedule);
        }

        private void RemoveOldReference(ReferenceType referenceType)
        {
            IList<Reference> oldReferences = Context.References.Where(x => x.ReferenceType == referenceType).ToList();

            if (oldReferences != null)
            {
                foreach (var reference in oldReferences)
                {
                    Context.References.Remove(reference);
                }
                Context.SaveChanges();
            }
        }

        public ActionResult NewsLetter()
        {
            var tvSchedulesViewModel = new ListReferencesViewModel
            {
                References = Context.References.Where(x => x.ReferenceType == ReferenceType.NewsLetter).ToList()
            };

            return View(tvSchedulesViewModel);
        }

        [HttpPost]
        public ActionResult UploadImage()
        {
            string _path = string.Empty;
            HttpPostedFileBase file = Request.Files[0];
            try
            {
                if (file.ContentLength > 0)
                {
                    string fileName = Path.GetFileName(file.FileName);
                    if (ValidExtention(fileName))
                    {
                        _path = Path.Combine(Server.MapPath("~/UploadedFiles"), fileName);
                        //file.SaveAs(_path);
                    }
                }
            }
            catch
            {
                ViewBag.Message = "File upload failed!!";
            }

            return Json(new { isSuccess = true, _path });
        }

        private static bool ValidExtention(string fileName)
        {
            string ext = Path.GetExtension(fileName).ToLowerInvariant();
            return ext == ".jpg" || ext == ".png" || ext == ".jpeg";
        }
    }
}