namespace TicketDesk.Service.Interface
{
    using System;
    using System.Collections.Generic;
    using Core.Model;
    using Criteria.FileManager;
    using Domain.Model;

    public interface IFileManagerService
    {
        CommandResult AddFolder(AddFolderCriteria criteria);

        CommandResult DeleteFolder(DeleteFolderCriteria criteria);

        CommandResult UploadFile(UploadFileCriteria criteria);

        CommandResult DeleteFile(DeleteFileCriteria criteria);

        IEnumerable<string> CommitPendingAttachments(Ticket ticket, Guid tempId);
    }
}