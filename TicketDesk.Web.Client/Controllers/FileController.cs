namespace TicketDesk.Web.Client.Controllers
{
    using IO;
    using System;
    using System.Configuration;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Mvc;

    [RoutePrefix("file")]
    [TdAuthorize(Roles = "TdInternalUsers,TdHelpDeskUsers,TdAdministrators")]
    public class FileController : Controller
    {
        [HttpPost]
        [Route("upload")]
        public async Task<ActionResult> Upload(Guid tempId)
        {
            var demoMode = (ConfigurationManager.AppSettings["ticketdesk:DemoModeEnabled"] ?? "false")
                .Equals("true", StringComparison.InvariantCultureIgnoreCase);
            if (demoMode)
            {
                return new EmptyResult();
            }

            // TODO: doesn't necessarily play well with large files, cap file upload size to something reasonable (define in settings) or use upload chunking
            foreach (string fileName in Request.Files)
            {
                var file = Request.Files[fileName];
                Debug.Assert(file != null, "file != null");

                await TicketDeskFileStore.SaveAttachmentAsync(file.InputStream, file.FileName, tempId.ToString(), true).ConfigureAwait(false);
            }

            // dropzone expects a message property back
            return new JsonCamelCaseResult { Data = new { Message = string.Empty } };
        }

        [HttpPost]
        [Route("delete")]
        public JsonResult Delete(Guid id, string fileName)
        {
            // ticketdesk never lets the UI directly delete a file attachment unless it is a pending.
            TicketDeskFileStore.DeleteAttachment(fileName, id.ToString(), true);

            // dropzone expects a message property back
            return new JsonCamelCaseResult { Data = new { Message = string.Empty } };
        }

        [HttpGet]
        [Route("{id}/{fileName}", Name = "GetAttachedFile")]
        public ActionResult GetFile(string id, string fileName)
        {
            return FetchFile(fileName, id, IsFilePending(id));
        }

        [HttpGet]
        [Route("get-attachments-info")]
        public ActionResult GetAttachmentsInfo(int? id, Guid? tempId)
        {
            var pending = tempId.HasValue ? TicketDeskFileStore.ListAttachmentInfo(tempId.Value.ToString(), true) : new TicketDeskFileInfo[0];
            var attached = id.HasValue ? TicketDeskFileStore.ListAttachmentInfo(id.Value.ToString(CultureInfo.InvariantCulture), false) : new TicketDeskFileInfo[0];

            var files = pending.Select(f => tempId != null ? new
            {
                f.Name,
                f.Size,
                isAttached = false,
                Type = MimeTypeMap.GetMimeType(Path.GetExtension(f.Name)),
                Url = Url.RouteUrl("GetPendingFile", new { Id = tempId.Value, FileName = f.Name, })
            } : null)
            .Union(attached.Select(f => id != null ? new
            {
                f.Name,
                f.Size,
                isAttached = true,
                Type = MimeTypeMap.GetMimeType(Path.GetExtension(f.Name)),
                Url = Url.RouteUrl("GetAttachedFile", new { Id = id.Value, FileName = f.Name })
            } : null));

            return new JsonCamelCaseResult
            {
                Data = files,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        private static bool IsFilePending(string containerId)
        {
            Guid tId;
            return Guid.TryParse(containerId, out tId);
        }

        private static ActionResult FetchFile(
            string fileName,
            string container,
            bool isPending,
            bool isDownload = false)
        {
            var fstream = TicketDeskFileStore.GetFile(fileName, container, isPending);

            if (isDownload)
            {
                // always send it back as octet-stream so client browser actually downloads it, instead of displaying it on-screen
                return new FileStreamResult(fstream, "application/octet-stream");
            }

            // display in browser
            string contentType = MimeMapping.GetMimeMapping(fileName);
            return new FileStreamResult(fstream, contentType);
        }
    }
}