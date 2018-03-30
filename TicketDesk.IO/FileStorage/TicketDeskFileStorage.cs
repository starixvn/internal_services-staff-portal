namespace TicketDesk.IO.FileStorage
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web;
    using AzureBlobFileSystem;

    public static class TicketDeskFileStore
    {
        private static readonly string Dir = Path.Combine(
            AppDomain.CurrentDomain.GetData("DataDirectory").ToString(),
            "filestore");

        private static IStorageProvider _currentProvider;

        private static IStorageProvider Current => _currentProvider ?? (_currentProvider = new FileSystemStorageProvider(Dir));

        public static bool MoveFile(string fileName, string oldContainerId, string newContainerId, bool wasPending,
            bool isPending)
        {
            var oldPath = GetFilePath(fileName, oldContainerId, wasPending);
            ////var oldFolder = GetFileFolderPath(oldContainerId, wasPending);
            var newPath = GetFilePath(fileName, newContainerId, isPending);
            if (Current.FileExists(oldPath))
            {
                if (!isPending)
                {
                    // don't care about return value
                    Current.TryCreateFolder(GetFileFolderPath(newContainerId, false));
                }

                // hadle case where filename already exists ; add (x) to filename
                var count = 1;
                var fileNameOnly = Path.GetFileNameWithoutExtension(fileName);
                var extension = Path.GetExtension(fileName);
                while (Current.FileExists(newPath))
                {
                    var tempFileName = $"{fileNameOnly}({count++}){extension}";
                    newPath = GetFilePath(tempFileName, newContainerId, isPending);
                }

                Current.RenameFile(oldPath, newPath);

                return true;
            }

            return false;
        }

        public static Stream GetFile(string fileName, string containerId, bool isPending)
        {
            var f = Current.GetFile(GetFilePath(fileName, containerId, isPending));
            return f.OpenRead();
        }

        public static IEnumerable<TicketDeskFileInfo> ListAttachmentInfo(string containerId, bool isPending)
        {
            var path = GetFileFolderPath(containerId, isPending);
            var files = Current.ListFiles(path);
            if (isPending)
            {
                files = files.Where(f => f.GetName().EndsWith(containerId));
            }

            return files.Select(f => new TicketDeskFileInfo
            {
                Name = TrimIdFromName(f.GetName(), containerId),
                Size = f.GetSize()
            });
        }

        public static async Task<bool> SaveAttachmentAsync(
            Stream inputStream,
            string fileName,
            string containerId,
            bool isPending)
        {
            var path = GetFilePath(fileName, containerId, isPending);
            using (var writer = new StreamWriter(Current.CreateFile(path).OpenWrite()))
            {
                await inputStream.CopyToAsync(writer.BaseStream).ConfigureAwait(false);
            }

            return true;
        }

        public static bool DeleteAttachment(string fileName, string containerId, bool isPending)
        {
            var path = GetFilePath(fileName, containerId, isPending);

            if (Current.FileExists(path))
            {
                Current.DeleteFile(path);
            }

            return true;
        }

        public static bool FileExists(string fileName, string containerId, bool isPending)
        {
            return Current.FileExists(GetFilePath(fileName, containerId, isPending));
        }

        public static async Task UploadFiles(IEnumerable<HttpPostedFileBase> files, int ticketId)
        {
            foreach (var file in files)
            {
                if (file != null)
                {
                    await SaveAttachmentAsync(file.InputStream, file.FileName, ticketId.ToString(), false)
                        .ConfigureAwait(false);
                }
            }
        }

        private static string GetFilePath(string fileName, string containerId, bool isPending)
        {
            // 260 is the upper limit for full file path
            // 74 is the length of 'ticketdesk-attachments\\pending\\' character to combine directory path and file name to full file path, plus guid at the end

            int characterLimit = 260 - Dir.Length - 74;

            string asciiFileName = RemoveDiacritics(fileName);

            string extension = asciiFileName.Split('.').Last();
            string name = asciiFileName.Substring(0, asciiFileName.LastIndexOf('.'));

            if(name.Length > characterLimit)
            {
                name = name.Substring(0, characterLimit);
            }

            var validFileName = isPending ? $"{name}.{extension}.{containerId}" : $"{name}.{extension}";

            var path = GetFileFolderPath(containerId, isPending);
            return Current.Combine(path, validFileName);
        }

        private static string TrimIdFromName(string fileName, string containerId)
        {
            return fileName.EndsWith(containerId)
                ? fileName.Substring(0, fileName.Length - (containerId.Length + 1))
                : fileName;
        }

        private static string GetFileFolderPath(string containerId, bool isPending)
        {
            var path = Current.Combine("ticketdesk-attachments", isPending ? "pending" : containerId);
            return path;
        }

        public static string RemoveDiacritics(string text)
        {
            string normalized = text.Normalize(NormalizationForm.FormKD);
            normalized = normalized.Replace('&', '_');
            Encoding removal = Encoding.GetEncoding(
                Encoding.ASCII.CodePage,
                new EncoderReplacementFallback(""),
                new DecoderReplacementFallback(""));
            byte[] bytes = removal.GetBytes(normalized);
            return Encoding.ASCII.GetString(bytes);
        }
    }
}