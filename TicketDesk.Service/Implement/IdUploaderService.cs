namespace TicketDesk.Service.Implement
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using Domain.Model;
    using Interface;

    public class IdUploaderService : EntityFrameworkBaseService, IIdUploaderService
    {
        public bool CreateCustomer(UploadedUserId uploadedUserId)
        {
            context.UploadedUserIds.Add(uploadedUserId);
            var result = context.SaveChanges();
            return result > 0;
        }

        public void MoveFiles(Guid tempId, string pendingFolder, string idUploadedFolder)
        {
            // Create or open folder with the record id
            var commitedFolder = Directory.CreateDirectory(idUploadedFolder);

            string fileEnding = tempId.ToString();

            try
            {
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
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }
    }
}
