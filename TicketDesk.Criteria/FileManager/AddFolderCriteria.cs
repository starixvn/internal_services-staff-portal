namespace TicketDesk.Criteria.FileManager
{
    using Fanex.Data.Repository;

    public class AddFolderCriteria : CriteriaBase
    {
        public int UserId { get; set; }

        public string FolderName { get; set; }

        public string Directory { get; set; }

        public override bool IsValid() => UserId > 0 && !string.IsNullOrWhiteSpace(FolderName) && !string.IsNullOrWhiteSpace(Directory);

        public override string GetSettingKey() => "DMS_Folder_Insert";
    }
}