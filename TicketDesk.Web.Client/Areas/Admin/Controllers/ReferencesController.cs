namespace TicketDesk.Web.Client.Areas.Admin.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Web;
    using System.Web.Configuration;
    using System.Web.Mvc;
    using Client.Controllers;
    using Core.Enumerations;
    using Core.Resources;
    using Domain;
    using Domain.Infrastructure;
    using Domain.Model;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Web.Attribute;
    using Models.References;

    public class ReferencesController : BaseController
    {
        private readonly string webRootPath;
        private readonly string webPath = WebConfigurationManager.AppSettings["FileManager_RootPath"];
        private readonly int imageSizeLimit = 2 * 1024 * 1024; // 2MB

        private TdDomainContext Context { get; }

        public ReferencesController(TdDomainContext context)
        {
            webRootPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, webPath);
            Context = context;

            if (!Directory.Exists(webRootPath))
            {
                Directory.CreateDirectory(webRootPath);
            }
        }

        public ActionResult TVSchedules()
        {
            var viewModel = GetListReferences(ReferenceType.TvSchedule);
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [NapAuth(FunctionKey = FunctionKeys.TicketCenter, Policy = Constant.Policy.Read)]
        public ActionResult TVSchedules(ListReferencesViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                if (CheckIfDateDuplicate(viewModel.References))
                {
                    ModelState.AddModelError("", Messages.DuplicatedDay);
                }
                else
                {
                    InsertListReference(ReferenceType.TvSchedule, viewModel);
                    TempData["Success"] = Messages.UpdateReferencesSuccessful;
                }
            }

            return View(viewModel);
        }

        public ActionResult FeatureEvents()
        {
            var viewModel = GetListReferences(ReferenceType.FeaturedEvent);
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [NapAuth(FunctionKey = FunctionKeys.TicketCenter, Policy = Constant.Policy.Read)]
        public ActionResult FeatureEvents(ListReferencesViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                if (CheckIfDateDuplicate(viewModel.References))
                {
                    ModelState.AddModelError("", Messages.DuplicatedDay);
                }
                else
                {
                    InsertListReference(ReferenceType.FeaturedEvent, viewModel);
                    TempData["Success"] = Messages.UpdateReferencesSuccessful;
                }
            }

            return View(viewModel);
        }

        public ActionResult BetOfTheDay()
        {
            var viewModel = GetListReferences(ReferenceType.BetOfTheDay);
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [NapAuth(FunctionKey = FunctionKeys.TicketCenter, Policy = Constant.Policy.Read)]
        public ActionResult BetOfTheDay(ListReferencesViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                if (CheckIfDateDuplicate(viewModel.References))
                {
                    ModelState.AddModelError("", Messages.DuplicatedDay);
                }
                else
                {
                    InsertListReference(ReferenceType.BetOfTheDay, viewModel);
                    TempData["Success"] = Messages.UpdateReferencesSuccessful;
                }
            }

            return View(viewModel);
        }

        public ActionResult TradingLimits()
        {
            var viewModel = GetListReferences(ReferenceType.TradingLimits);
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [NapAuth(FunctionKey = FunctionKeys.TicketCenter, Policy = Constant.Policy.Read)]
        public ActionResult TradingLimits(ListReferencesViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                if (CheckIfDateDuplicate(viewModel.References))
                {
                    ModelState.AddModelError("", Messages.DuplicatedDay);
                }
                else
                {
                    InsertListReference(ReferenceType.TradingLimits, viewModel);
                    TempData["Success"] = Messages.UpdateReferencesSuccessful;
                }
            }

            return View(viewModel);
        }

        public ActionResult NewsLetter()
        {
            var viewModel = GetListReferences(ReferenceType.NewsLetter);
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [NapAuth(FunctionKey = FunctionKeys.TicketCenter, Policy = Constant.Policy.Read)]
        public ActionResult NewsLetter(ListReferencesViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                InsertListReference(ReferenceType.NewsLetter, viewModel);
                TempData["Success"] = Messages.UpdateReferencesSuccessful;
            }

            return View(viewModel);
        }

        public ActionResult GetReferenceEntity(bool isNewsLetter)
        {
            var referenceType = isNewsLetter ? ReferenceType.NewsLetter : ReferenceType.TvSchedule;
            var reference = new ReferenceViewModel
            {
                ReferenceType = referenceType
            };
            return PartialView("_ReferenceEntryEditor", reference);
        }

        private void InsertListReference(ReferenceType referenceType, ListReferencesViewModel listReferences)
        {
            var references = listReferences.References;
            RemoveOldReference(referenceType);
            if (references != null && references.Count > 0)
            {
                foreach (ReferenceViewModel reference in references)
                {
                    SaveReference(reference, referenceType);
                }
            }

            Context.SaveChanges();
        }

        private ListReferencesViewModel GetListReferences(ReferenceType referenceType)
        {
            var references = Context.References.Where(x => x.ReferenceType == referenceType).ToList();
            if (referenceType != ReferenceType.NewsLetter)
            {
                references = references.Where(r => DateTime.Compare(r.DateAppear.Date, DateTime.Now.Date) >= 0).ToList();
            }

            var listViewModel = new List<ReferenceViewModel>();

            foreach (Reference reference in references)
            {
                var referenceViewModel = new ReferenceViewModel
                {
                    DateAppear = reference.DateAppear,
                    ImageLink = reference.ImageLink,
                    ReferenceType = reference.ReferenceType
                };
                listViewModel.Add(referenceViewModel);
            }

            return new ListReferencesViewModel { References = listViewModel };
        }

        private void SaveReference(ReferenceViewModel reference, ReferenceType referenceType)
        {
            var data = new Reference
            {
                DateAppear = referenceType != ReferenceType.NewsLetter ? reference.DateAppear : DateTime.Now,
                ImageLink = reference.ImageLink,
                ReferenceType = referenceType,
                LastUpdateBy = CurrentSession.Username,
                LastUpdateDate = DateTime.Now
            };

            Context.References.Add(data);
        }

        private void RemoveOldReference(ReferenceType referenceType)
        {
            IList<Reference> oldReferences = Context.References.Where(x => x.ReferenceType == referenceType).ToList();
            if (referenceType != ReferenceType.NewsLetter)
            {
                oldReferences = oldReferences.Where(r => DateTime.Compare(r.DateAppear.Date, DateTime.Now.Date) >= 0).ToList();
            }

            if (oldReferences != null)
            {
                foreach (var reference in oldReferences)
                {
                    Context.References.Remove(reference);
                }
                Context.SaveChanges();
            }
        }

        [HttpPost]
        public ActionResult UploadImage()
        {
            HttpPostedFileBase file = Request.Files[0];
            var filePath = string.Empty;
            bool isSuccessful = false;

            if (file.ContentLength > 0)
            {
                var domain = Request.Url.GetLeftPart(UriPartial.Authority);
                var today = DateTime.Now.Date;
                var directory = string.Format("{0}/{1}/{2}/{3}", webRootPath, today.Year, today.Month, today.Day);
                var imageDirectory = string.Format("{0}/{1}/{2}/{3}/{4}", domain, webPath, today.Year, today.Month, today.Day);
                string fileName = Path.GetFileName(file.FileName);
                int fileSize = file.ContentLength;

                if (!ValidExtention(fileName))
                {
                    return Json(new { isSuccessful, message = Messages.InvalidImageFormat });
                }

                if (fileSize > imageSizeLimit)
                {
                    return Json(new { isSuccessful, message = Messages.ImageExceededCapacity });
                }

                var fileExist = System.IO.File.Exists(Path.Combine(directory, file.FileName));
                filePath = MakeWebPath(Path.Combine(directory, file.FileName));

                if (!fileExist)
                {
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }

                    file.SaveAs(filePath);
                }

                imageDirectory = string.Format("{0}/{1}", imageDirectory, fileName);

                return Json(new { isSuccessful = true, imageDirectory });
            }

            return Json(new { isSuccessful, message = "Upload fail" });
        }

        [AllowAnonymous]
        public ActionResult IsValidImageUrl()
        {
            var keyQuery = Request.QueryString.GetKey(0);
            var value = Request.QueryString[keyQuery];
            bool validUrl = true;
            try
            {
                var imageLink = new Uri(value);
                var req = WebRequest.Create(imageLink);
                WebResponse response = req.GetResponse();
                validUrl = CheckValidImageUrlExtension(response, "jpeg") || CheckValidImageUrlExtension(response, "jpg") || CheckValidImageUrlExtension(response, "png");
            }
            catch
            {
                validUrl = false;
            }
            return Json(validUrl, JsonRequestBehavior.AllowGet);
        }

        private static bool CheckValidImageUrlExtension(WebResponse response, string extension)
        {
            return response.ContentType.ToLower(CultureInfo.InvariantCulture)
                               .StartsWith(string.Format("image/{0}", extension));
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

        private static bool CheckIfDateDuplicate(IList<ReferenceViewModel> references)
        {
            if (references != null)
            {
                var duplicates = references.GroupBy(r => r.DateAppear.Date)
                                       .Where(g => g.Count() > 1)
                                       .Select(g => g.Key);
                return duplicates.Any();
            }
            return false;
        }
    }
}