namespace TicketDesk.Web.Client.Controllers
{
    using Domain;
    using Domain.Model;
    using Models;
    using System;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Web.Mvc;

    public class IdUploaderController : BaseController
    {
        private TdDomainContext Context { get; }

        public IdUploaderController(TdDomainContext context)
        {
            Context = context;
        }

        // GET: IdUploader
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

        //[NapAuth(FunctionKey = FunctionKeys., Policy = Constant.Policy.Read)]
        [HttpPost]
        [ValidateAntiForgeryToken]
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

                Context.UploadedUserIds.Add(uploadedUserId);

                Context.SaveChanges();

                MoveFiles(uploadedUserId.Id.ToString(), viewModel.TempId);
            }

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        private void MoveFiles(string folderName, Guid tempId)
        {
            // Create or open folder with the record id
            var commitedFolder = CreateFolder(folderName);

            // Get pending folder path
            var pendingFolder = Server.MapPath("~/App_Data/filestore/ticketdesk-attachments/pending");

            string fileEnding = tempId.ToString();
            
            // Get all file belongs to that record
            var files = Directory.EnumerateFiles(pendingFolder)
                .Where(x => x.EndsWith(fileEnding));

            // Get names of the files
            var names = files.Select(x => x.Replace(pendingFolder, string.Empty).TrimStart('\\')).ToList();

            // Move the file from pending folder to commited folder
            foreach (string file in names)
            {
                string fileToMove = file.Replace(fileEnding, string.Empty).TrimEnd('.');

                Directory.Move(
                    Path.Combine(pendingFolder, file), 
                    Path.Combine(commitedFolder.FullName, fileToMove));
            }
        }

        private DirectoryInfo CreateFolder(string folderName)
        {
            var folder = Server.MapPath("~/App_Data/filestore/id-uploaded/" + folderName);
            return Directory.CreateDirectory(folder);
        }
    }
}