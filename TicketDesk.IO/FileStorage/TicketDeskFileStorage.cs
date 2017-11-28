namespace TicketDesk.IO
{
    using AzureBlobFileSystem;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web;

    public static class TicketDeskFileStore
    {
        private static IStorageProvider _currentProvider;

        private static IStorageProvider Current
        {
            get
            {
                if (_currentProvider == null)
                {
                    var accout = AzureConnectionHelper.CloudStorageAccount;
                    if (accout != null)
                    {
                        _currentProvider = new AzureBlobStorageProvider(accout);
                    }
                    else
                    {
                        var dir = Path.Combine(AppDomain.CurrentDomain.GetData("DataDirectory").ToString(), "filestore");
                        _currentProvider = new FileSystemStorageProvider(dir);
                    }
                }

                return _currentProvider;
            }
        }

        public static bool MoveFile(string fileName, string oldContainerId, string newContainerId, bool wasPending, bool isPending)
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

        public static async Task<bool> SaveAttachmentAsync(Stream inputStream, string fileName, string containerId, bool isPending)
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
            string asciiFileName = RemoveDiacritics(fileName);
            var validFileName = isPending ? $"{asciiFileName}.{containerId}" : asciiFileName;

            var path = GetFileFolderPath(containerId, isPending);
            return Current.Combine(path, validFileName);
        }

        private static string TrimIdFromName(string fileName, string containerId)
        {
            return fileName.EndsWith(containerId) ? fileName.Substring(0, fileName.Length - (containerId.Length + 1)) : fileName;
        }

        private static string GetFileFolderPath(string containerId, bool isPending)
        {
            var path = Current.Combine("ticketdesk-attachments", isPending ? "pending" : containerId);
            return path;
        }

        public static string RemoveDiacritics(string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }
    }
}