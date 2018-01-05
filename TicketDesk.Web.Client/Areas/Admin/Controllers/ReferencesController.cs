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
    using TicketDesk.Web.Client.Areas.Admin.Models.References;

    public class ReferencesController : Controller
    {
        private readonly TdDomainContext Context = new TdDomainContext();
        private readonly string webRootPath;
        private readonly int imageSizeLimit = 2 * 1024 * 1024; // 2MB

        public ReferencesController()
        {
            string webPath = WebConfigurationManager.AppSettings["FileManager_RootPath"];
            webRootPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, webPath);

            if (!Directory.Exists(webRootPath))
            {
                Directory.CreateDirectory(webRootPath);
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
        public ActionResult TVSchedules(IEnumerable<Reference> references)
        {
            if (ModelState.IsValid)
            {
                RemoveOldReference(ReferenceType.TvSchedule);

                foreach (Reference reference in references)
                {
                    SaveEachReference(reference, ReferenceType.TvSchedule);
                }
                Context.SaveChanges();
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
                DateAppear = referenceType != ReferenceType.NewsLetter ? reference.DateAppear : DateTime.Now,
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
            HttpPostedFileBase file = Request.Files[0];
            var filePath = string.Empty;
            bool isSuccessful = false;

            if (file.ContentLength > 0)
            {
                var today = DateTime.Now.Date;
                var directory = string.Format("{0}/{1}/{2}/{3}", webRootPath, today.Year, today.Month, today.Day);
                string fileName = Path.GetFileName(file.FileName);
                int fileSize = file.ContentLength;

                if (!ValidExtention(fileName))
                {
                    return Json(new { isSuccessful, message = "Fail to upload image. The format is not supported." });
                }

                if (fileSize > imageSizeLimit)
                {
                    return Json(new { isSuccessful, message = "Fail to upload image. The image you're trying to upload exceeds 2MB limit." });
                }

                var fileExist = System.IO.File.Exists(Path.Combine(directory, file.FileName));

                if (fileExist)
                {
                    filePath = MakeWebPath(Path.Combine(directory, file.FileName));
                }
                else
                {
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }

                    filePath = MakeWebPath(Path.Combine(directory, file.FileName));
                    file.SaveAs(filePath);
                }

                return Json(new { isSuccessful = true, filePath });
            }

            return Json(new { isSuccessful, message = "Upload fail" });
        }

        private static bool ValidExtention(string fileName)
        {
            string ext = Path.GetExtension(fileName).ToLowerInvariant();
            return ext == ".jpg" || ext == ".png" || ext == ".jpeg";
        }

        private static string MakeWebPath(string path, bool addSeperatorToBegin = false, bool addSeperatorToLast = false)
        {
            var validPath = path.Replace("\\", "/");

            if (addSeperatorToBegin)
            {
                validPath = "/" + validPath;
            }

            if (addSeperatorToLast)
            {
                validPath = validPath + "/";
            }

            return validPath;
        }
    }
}