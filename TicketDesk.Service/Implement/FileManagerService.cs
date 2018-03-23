namespace TicketDesk.Service.Implement
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Core.Model;
    using Core.Services;
    using Criteria.FileManager;
    using Domain.Model;
    using Interface;
    using IO;

    public class FileManagerService : BaseService, IFileManagerService
    {
        public CommandResult AddFolder(AddFolderCriteria criteria)
        {
            var commandResult = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                commandResult = Repository.ExecuteNonQuery(criteria);
            }

            return commandResult;
        }

        public CommandResult DeleteFolder(DeleteFolderCriteria criteria)
        {
            var commandResult = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                commandResult = Repository.ExecuteNonQuery(criteria);
            }

            return commandResult;
        }

        public CommandResult UploadFile(UploadFileCriteria criteria)
        {
            var commandResult = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                commandResult = Repository.ExecuteNonQuery(criteria);
            }

            return commandResult;
        }

        public CommandResult DeleteFile(DeleteFileCriteria criteria)
        {
            var commandResult = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                commandResult = Repository.ExecuteNonQuery(criteria);
            }

            return commandResult;
        }

        public IEnumerable<string> CommitPendingAttachments(Ticket ticket, Guid tempId)
        {
            var attachments = TicketDeskFileStore.ListAttachmentInfo(tempId.ToString(), true).ToArray();

            foreach (var attachment in attachments)
            {
                TicketDeskFileStore.MoveFile(
                    attachment.Name,
                    tempId.ToString(),
                    ticket.TicketId.ToString(CultureInfo.InvariantCulture),
                    true,
                    false);
            }

            return attachments.Select(a => a.Name);
        }
    }
}