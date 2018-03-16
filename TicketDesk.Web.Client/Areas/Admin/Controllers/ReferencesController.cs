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
    using Domain.Infrastructure;
    using Domain.Model;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Web.Attribute;
    using Models.References;
    using Service.Interface;

    public class ReferencesController : BaseController
    {
        private readonly string webRootPath;
        private readonly string webPath = WebConfigurationManager.AppSettings["FileManager_RootPath"];
        private readonly int imageSizeLimit = 2 * 1024 * 1024; // 2MB
        private readonly int limitHeight = 400;
        private readonly int limitWidth = 600;

        private readonly IReferenceService _referenceService;

        public ReferencesController(IReferenceService referenceService)
        {
            webRootPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, webPath);
            _referenceService = referenceService;

            if (!Directory.Exists(webRootPath))
            {
                Directory.CreateDirectory(webRootPath);
            }
        }

        public ActionResult TVSchedules()
        {
            var viewModel = GetListReferenceViewModel(ReferenceType.TvSchedule);
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [NapAuth(FunctionKey = FunctionKeys.References, Policy = Constant.Policy.Host)]
        public JsonResult TVSchedules(ListReferencesViewModel viewModel)
        {
            return HandlePostReferences(ReferenceType.TvSchedule, viewModel);
        }

        public ActionResult FeatureEvents()
        {
            var viewModel = GetListReferenceViewModel(ReferenceType.FeaturedEvent);
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [NapAuth(FunctionKey = FunctionKeys.References, Policy = Constant.Policy.Host)]
        public JsonResult FeatureEvents(ListReferencesViewModel viewModel)
        {
            return HandlePostReferences(ReferenceType.FeaturedEvent, viewModel);
        }

        public ActionResult BetOfTheDay()
        {
            var viewModel = GetListReferenceViewModel(ReferenceType.BetOfTheDay);
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [NapAuth(FunctionKey = FunctionKeys.References, Policy = Constant.Policy.Host)]
        public JsonResult BetOfTheDay(ListReferencesViewModel viewModel)
        {
            return HandlePostReferences(ReferenceType.BetOfTheDay, viewModel);
        }

        public ActionResult TradingLimits()
        {
            var viewModel = GetListReferenceViewModel(ReferenceType.TradingLimits);
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [NapAuth(FunctionKey = FunctionKeys.References, Policy = Constant.Policy.Host)]
        public JsonResult TradingLimits(ListReferencesViewModel viewModel)
        {
            return HandlePostReferences(ReferenceType.TradingLimits, viewModel);
        }

        public ActionResult Newsletters()
        {
            var viewModel = GetListReferenceViewModel(ReferenceType.Newsletters);
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [NapAuth(FunctionKey = FunctionKeys.References, Policy = Constant.Policy.Host)]
        public ActionResult Newsletters(ListReferencesViewModel viewModel)
        {
            return HandlePostReferences(ReferenceType.Newsletters, viewModel);
        }

        private JsonResult HandlePostReferences(ReferenceType referenceType, ListReferencesViewModel viewModel)
        {
            var isSuccessful = false;
            var message = string.Empty;

            if (referenceType != ReferenceType.Newsletters && CheckIfDateDuplicate(viewModel.References))
            {
                ModelState.AddModelError("", Messages.DuplicatedDay);
            }

            if (ModelState.IsValid)
            {
                HandleListReferenceUploaded(referenceType, viewModel);
                isSuccessful = true;
                message = Messages.UpdateReferencesSuccessful;
            }

            var errors = from item in ModelState
                         where item.Value.Errors.Any()
                         select new { item.Key, item.Value.Errors.FirstOrDefault().ErrorMessage };

            return Json(new { isSuccessful, message, errors });
        }

        [NapAuth(FunctionKey = FunctionKeys.References, Policy = Constant.Policy.Host)]
        public ActionResult GetReferenceEntity(bool isNewsletters)
        {
            var referenceType = isNewsletters ? ReferenceType.Newsletters : ReferenceType.TvSchedule;
            var reference = new ReferenceViewModel
            {
                ReferenceType = referenceType
            };
            return PartialView("_ReferenceEntryEditor", reference);
        }

        private void HandleListReferenceUploaded(ReferenceType referenceType, ListReferencesViewModel listReferences)
        {
            var criteriaList = listReferences.GetQueryCriteria();
            _referenceService.UploadReferences(referenceType, criteriaList);
        }

        private ListReferencesViewModel GetListReferenceViewModel(ReferenceType referenceType)
        {
            var references = _referenceService.GetListReference(referenceType);

            var listReference = new List<ReferenceViewModel>();

            foreach (Reference reference in references)
            {
                var referenceViewModel = new ReferenceViewModel
                {
                    Id = reference.Id,
                    DateAppear = reference.DateAppear,
                    ImageLink = reference.ImageLink,
                    ReferenceType = reference.ReferenceType
                };
                listReference.Add(referenceViewModel);
            }

            var viewModel = new ListReferencesViewModel
            {
                IsHostPermission = IsHaveHostPermission(FunctionKeys.References, Constant.Policy.Host),
                References = referenceType == ReferenceType.Newsletters ? listReference : listReference.OrderBy(r => r.DateAppear).ToList()
            };

            return viewModel;
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

                if (!IsValidExtention(fileName))
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

        private static bool IsValidExtention(string fileName)
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
                return references.GroupBy(r => r.DateAppear.Date)
                                       .Where(g => g.Count() > 1)
                                       .Select(g => g.Key).Any();
            }
            return false;
        }
    }
}