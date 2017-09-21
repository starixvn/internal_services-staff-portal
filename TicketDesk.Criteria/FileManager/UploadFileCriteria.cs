namespace TicketDesk.Criteria.FileManager
{
    using Fanex.Data.Repository;

    public class UploadFileCriteria : CriteriaBase
    {
        public int UserId { get; set; }

        public string FileName { get; set; }

        public int FileSize { get; set; }

        public string ContentType { get; set; }

        public string Directory { get; set; }

        public override bool IsValid() => UserId > 0 && !string.IsNullOrWhiteSpace(FileName) && !string.IsNullOrWhiteSpace(Directory);

        public override string GetSettingKey() => "DMS_File_Insert";
    }
}