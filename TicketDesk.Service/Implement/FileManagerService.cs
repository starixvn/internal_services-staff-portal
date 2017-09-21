namespace TicketDesk.Service
{
    using Core.Model;
    using Core.Services;
    using Criteria.FileManager;

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
    }
}