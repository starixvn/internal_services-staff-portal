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
    using Localization.Client;
    using Models.References;
    using Service.Interface;

    [NapAuth(FunctionKey = FunctionKeys.ReferenceAdmin, Policy = Constant.Policy.Read)]
    public class ReferencesController : BaseController
    {
        private readonly string webRootPath;
        private readonly string webPath = WebConfigurationManager.AppSettings["FileManager_RootPath"];
        private readonly int imageSizeLimit = 2 * 1024 * 1024; // 2MB
        private readonly int limitHeight = 400;
        private readonly int limitWidth = 600;

        private readonly IReferenceService referenceService;

        public ReferencesController(IReferenceService referenceService)
        {
            webRootPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, webPath);
            this.referenceService = referenceService;

            if (!Directory.Exists(webRootPath))
            {
                Directory.CreateDirectory(webRootPath);
            }
        }

        [NapAuth(FunctionKey = FunctionKeys.TvScheduleAdmin, Policy = Constant.Policy.Read)]
        public ActionResult TVSchedules()
        {
            var viewModel = GetListReferenceViewModel(ReferenceType.TvSchedule);
            return View(viewModel);
        }

        [NapAuth(FunctionKey = FunctionKeys.FeaturedEventAdmin, Policy = Constant.Policy.Read)]
        public ActionResult FeatureEvents()
        {
            var viewModel = GetListReferenceViewModel(ReferenceType.FeaturedEvent);
            return View(viewModel);
        }

        [NapAuth(FunctionKey = FunctionKeys.BetOfTheDayAdmin, Policy = Constant.Policy.Read)]
        public ActionResult BetOfTheDay()
        {
            var viewModel = GetListReferenceViewModel(ReferenceType.BetOfTheDay);
            return View(viewModel);
        }

        [NapAuth(FunctionKey = FunctionKeys.TradingLimitsAdmin, Policy = Constant.Policy.Read)]
        public ActionResult TradingLimits()
        {
            var viewModel = GetListReferenceViewModel(ReferenceType.TradingLimits);
            return View(viewModel);
        }

        [NapAuth(FunctionKey = FunctionKeys.NewslettersAdmin, Policy = Constant.Policy.Read)]
        public ActionResult Newsletters()
        {
            var viewModel = GetListReferenceViewModel(ReferenceType.Newsletters);
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateOnlyIncomingValues]
        [NapAuth(FunctionKey = FunctionKeys.ReferenceAdmin, Policy = Constant.Policy.Host)]
        public JsonResult UploadReferences(ReferencesViewModel viewModel)
        {
            var isSuccessful = false;
            var message = string.Empty;

            if (viewModel.Type != ReferenceType.Newsletters && CheckIfDateDuplicate(viewModel.References.ToList()))
            {
                ModelState.AddModelError("DateAppear", Messages.DuplicatedDay);
            }

            if (ModelState.IsValid)
            {
                HandleListReferenceUploaded(viewModel);
                isSuccessful = true;
                message = Messages.UpdateReferencesSuccessful;
            }

            var errors = from item in ModelState
                         where item.Value.Errors.Any()
                         select new { item.Key, item.Value.Errors.FirstOrDefault().ErrorMessage };

            return Json(new { isSuccessful, message, errors });
        }

        [NapAuth(FunctionKey = FunctionKeys.ReferenceAdmin, Policy = Constant.Policy.Host)]
        public ActionResult GetReferenceEntity(ReferenceType type)
        {
            var reference = new Reference
            {
                ReferenceType = type
            };

            return PartialView("_ReferenceEntryEditor", reference);
        }

        private void HandleListReferenceUploaded(ReferencesViewModel viewModel)
        {
            referenceService.UploadReferences(viewModel.Type, viewModel.References);
        }

        private ReferencesViewModel GetListReferenceViewModel(ReferenceType referenceType)
        {
            string functionKey = string.Format("{0}{1}", referenceType.ToString(), "Admin");
            var viewModel = new ReferencesViewModel
            {
                IsHostPermission = IsHavePermission(functionKey, Constant.Policy.Host),
                References = referenceService.GetListReference(referenceType),
                Type = referenceType
            };

            return viewModel;
        }

        [HttpPost]
        [NapAuth(FunctionKey = FunctionKeys.ReferenceAdmin, Policy = Constant.Policy.Host)]
        public ActionResult UploadImage(ReferenceType type)
        {
            HttpPostedFileBase file = Request.Files[0];

            if (file.ContentLength > 0)
            {
                if (!IsValidImageUpload(file))
                {
                    return Json(new { isSuccessful = false, message = GetErrorMessageUploadFile(file) });
                }

                var domain = Request.Url.GetLeftPart(UriPartial.Authority);
                var directory = string.Format("{0}/{1}", webRootPath, GetDirectoryToSaveFile(type));
                var imageUrl = string.Format("{0}/{1}/{2}", domain, webPath, GetDirectoryToSaveFile(type));
                string fileName = Path.GetFileName(file.FileName);

                MoveFileToDirectory(file, directory);

                imageUrl = string.Format("{0}/{1}", imageUrl, fileName);

                return Json(new { isSuccessful = true, imageUrl });
            }

            return Json(new { isSuccessful = false, message = Resources.UploadFailed });
        }

        private static string GetDirectoryToSaveFile(ReferenceType type)
        {
            var today = DateTime.Now;
            var timestamp = today.ToString("yyyyMMddHHmmssffff");
            return string.Format("{0}/{1}/{2}/{3}/{4}/{5}", type.ToString(), today.Year, today.Month, today.Day, today.Hour, timestamp);
        }

        private static void MoveFileToDirectory(HttpPostedFileBase file, string directory)
        {
            var fileExist = System.IO.File.Exists(Path.Combine(directory, file.FileName));
            string filePath = MakeWebPath(Path.Combine(directory, file.FileName));

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            if (fileExist)
            {
                System.IO.File.Delete(Path.Combine(directory, file.FileName));
            }

            file.SaveAs(filePath);
        }

        private bool IsValidImageUpload(HttpPostedFileBase file)
        {
            return IsValidFilenameExtension(Path.GetFileName(file.FileName)) &&
                   file.ContentLength <= imageSizeLimit &&
                   IsValidImageDimension(file);
        }

        private string GetErrorMessageUploadFile(HttpPostedFileBase file)
        {
            var message = string.Empty;
            if (!IsValidFilenameExtension(Path.GetFileName(file.FileName)))
            {
                message = Messages.InvalidImageFormat;
            }

            if (file.ContentLength > imageSizeLimit)
            {
                message = Messages.ImageExceededCapacity;
            }

            if (!IsValidImageDimension(file))
            {
                message = Messages.ImageDimensionInvalid;
            }

            return message;
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
                validUrl = IsValidImageUrlExtension(response, "jpeg") || IsValidImageUrlExtension(response, "jpg") || IsValidImageUrlExtension(response, "png");
            }
            catch
            {
                validUrl = false;
            }

            return Json(validUrl, JsonRequestBehavior.AllowGet);
        }

        private static bool IsValidImageUrlExtension(WebResponse response, string extension)
        {
            return response.ContentType.ToLower(CultureInfo.InvariantCulture)
                               .StartsWith(string.Format("image/{0}", extension), StringComparison.CurrentCulture);
        }

        private static bool IsValidFilenameExtension(string fileName)
        {
            string ext = Path.GetExtension(fileName).ToLowerInvariant();
            return ext == ".jpg" || ext == ".png" || ext == ".jpeg";
        }

        private bool IsValidImageDimension(HttpPostedFileBase file)
        {
            Stream fileStream = file.InputStream;
            Image image = Image.FromStream(fileStream);
            int imageHeight = image.Height;
            int imageWidth = image.Width;
            image.Dispose();

            return imageHeight >= limitHeight && imageWidth >= limitWidth;
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

        private static bool CheckIfDateDuplicate(IList<Reference> references)
        {
            return references != null && references.GroupBy(r => r.DateAppear.Date).Any(g => g.Count() > 1);
        }
    }
}