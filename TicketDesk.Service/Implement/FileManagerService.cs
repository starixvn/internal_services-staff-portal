namespace TicketDesk.Service.Implement
{
    using Core.Model;
    using Core.Services;
    using Criteria.FileManager;
    using Interface;

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
    }
}