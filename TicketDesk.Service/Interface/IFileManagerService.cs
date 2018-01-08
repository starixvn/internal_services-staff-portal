namespace TicketDesk.Service.Interface
{
    using Core.Model;
    using Criteria.FileManager;

    public interface IFileManagerService
    {
        CommandResult AddFolder(AddFolderCriteria criteria);

        CommandResult DeleteFolder(DeleteFolderCriteria criteria);

        CommandResult UploadFile(UploadFileCriteria criteria);

        CommandResult DeleteFile(DeleteFileCriteria criteria);
    }
}