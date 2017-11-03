namespace TicketDesk.Web.Client.Controllers
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text.RegularExpressions;
    using Models;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Domain;
    using Domain.Model;

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
                MaxNumberOfFile = 3,
                AcceptedExtensions = new[] { ".jpg", ".jpeg", ".png" }
            };

            return View(viewModel);
        }

        //[NapAuth(FunctionKey = FunctionKeys., Policy = Constant.Policy.Read)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> NewCustomer(IdUploaderViewModel viewModel)
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
            CreateFolder(folderName);

            var pendingFolder = Server.MapPath("~/App_Data/filestore/ticketdesk-attachments/pending");

            string fileEnding = tempId.ToString();

            var files = Directory.GetFiles(pendingFolder).Where(x => x.EndsWith(fileEnding));

            foreach (string file in files)
            {
                string fileToMove = Regex.Split(file, "." + fileEnding)[0];
                Directory.Move(file, Path.Combine(folderName, fileToMove));
            }
        }

        private DirectoryInfo CreateFolder(string folderName)
        {
            var folder = Server.MapPath("~/App_Data/filestore/id-uploaded/" + folderName);
            return Directory.CreateDirectory(folder);
        }


    }
}