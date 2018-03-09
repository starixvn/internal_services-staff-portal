namespace TicketDesk.Web.Client.Controllers
{
    using System;
    using System.Net;
    using System.Web.Mvc;
    using Domain.Infrastructure;
    using Domain.Model;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Web.Attribute;
    using Models;
    using Service.Interface;

    public class IdUploaderController : BaseController
    {
        private readonly IIdUploaderService idUploaderService;

        public IdUploaderController(IIdUploaderService idUploaderService)
        {
            this.idUploaderService = idUploaderService;
        }

        // GET: IdUploader
        [NapAuth(FunctionKey = FunctionKeys.IdUploader, Policy = Constant.Policy.Read)]
        public ActionResult Index()
        {
            IdUploaderViewModel viewModel = new IdUploaderViewModel
            {
                TempId = Guid.NewGuid(),
                MaxNumberOfFile = 3,
                AcceptedExtensions = new[] { ".jpg", ".jpeg", ".png" }
            };

            return View(viewModel);
        }

        [NapAuth(FunctionKey = FunctionKeys.IdUploader, Policy = Constant.Policy.Read)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult NewCustomer(IdUploaderViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                UploadedUserId uploadedUserId = new UploadedUserId
                {
                    CustomerUsername = viewModel.CustomerUserName,
                    CustomerName = viewModel.CustomerName,
                    IdNumber = viewModel.IdNumber,
                    Remark = viewModel.Remark
                };

                idUploaderService.CreateCustomer(uploadedUserId);

                string pendingFolder = Server.MapPath("~/App_Data/filestore/ticketdesk-attachments/pending");
                string idUploaderFolder = Server.MapPath("~/App_Data/filestore/id-uploaded/" + uploadedUserId.Id);

                idUploaderService.MoveFiles(viewModel.TempId, pendingFolder, idUploaderFolder);

                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }

            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
    }
}