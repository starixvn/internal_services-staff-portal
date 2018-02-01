namespace TicketDesk.Web.Client.Areas.Admin.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
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
        private readonly int limitHeight = 400;
        private readonly int limitWidth = 600;

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
                    HandleListReferenceUploaded(ReferenceType.TvSchedule, viewModel);
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
                    HandleListReferenceUploaded(ReferenceType.FeaturedEvent, viewModel);
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
                    HandleListReferenceUploaded(ReferenceType.BetOfTheDay, viewModel);
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
                    HandleListReferenceUploaded(ReferenceType.TradingLimits, viewModel);
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
                HandleListReferenceUploaded(ReferenceType.NewsLetter, viewModel);
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

        private void HandleListReferenceUploaded(ReferenceType referenceType, ListReferencesViewModel listReferences)
        {
            var references = listReferences.References;
            var originReferences = GetListReferences(referenceType);
            var oldReferences = references.Where(r => r.Id != null).ToList();

            if (references != null && references.Count > 0)
            {
                RemoveOldReferences(originReferences.References, oldReferences);
                foreach (ReferenceViewModel reference in references)
                {
                    if (reference.Id == null)
                    {
                        SaveReference(reference, referenceType);
                    }
                    else
                    {
                        UpdateReference(reference);
                    }
                }
            }
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
                    Id = reference.Id,
                    DateAppear = reference.DateAppear,
                    ImageLink = reference.ImageLink,
                    ReferenceType = reference.ReferenceType
                };
                listViewModel.Add(referenceViewModel);
            }

            return new ListReferencesViewModel { References = listViewModel };
        }

        private void RemoveOldReferences(IList<ReferenceViewModel> oldReferences, List<ReferenceViewModel> newReferences)
        {
            var excludedIDs = new HashSet<int?>(newReferences.Select(p => p.Id));
            var listDeletedReferences = oldReferences.Where(p => !excludedIDs.Contains(p.Id)).ToList();
            foreach (var reference in listDeletedReferences)
            {
                var referenceToDelete = Context.References.Where(r => r.Id == reference.Id).FirstOrDefault();
                if (referenceToDelete != null)
                {
                    Context.References.Remove(referenceToDelete);
                    Context.SaveChanges();
                }
            }
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
            Context.SaveChanges();
        }

        private void UpdateReference(ReferenceViewModel reference)
        {
            var oldReference = Context.References.Where(x => x.Id == reference.Id).FirstOrDefault();
            if (IsChangedReference(oldReference, reference))
            {
                oldReference.ImageLink = reference.ImageLink;
                oldReference.DateAppear = oldReference.ReferenceType != ReferenceType.NewsLetter ? reference.DateAppear : DateTime.Now;
                oldReference.LastUpdateDate = DateTime.Now;
                oldReference.LastUpdateBy = CurrentSession.Username;
            }
            Context.SaveChanges();
        }

        private static bool IsChangedReference(Reference oldReference, ReferenceViewModel newReference)
        {
            var isHaveChanged = false;
            var compareDateAppear = oldReference.ReferenceType == ReferenceType.NewsLetter || DateTime.Compare(oldReference.DateAppear.Date, newReference.DateAppear.Date) != 0;
            if (!compareDateAppear || !string.Equals(oldReference.ImageLink, newReference.ImageLink))
            {
                isHaveChanged = true;
            }
            return isHaveChanged;
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

                if (!ValidImageDimension(file))
                {
                    return Json(new { isSuccessful, message = Messages.ImageDimensionInvalid });
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

        private bool ValidImageDimension(HttpPostedFileBase file)
        {
            var isValid = false;
            Stream fileStream = file.InputStream;
            Image image = Image.FromStream(fileStream);
            int imageHeight = image.Height;
            int imageWidth = image.Width;
            image.Dispose();

            if (imageHeight >= limitHeight && imageWidth >= limitWidth)
            {
                isValid = true;
            }

            return isValid;
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