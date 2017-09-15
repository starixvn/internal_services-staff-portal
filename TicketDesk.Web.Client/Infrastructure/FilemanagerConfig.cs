namespace TicketDesk.Web.Client
{
    using System.Collections.Generic;
    using System.Web.Configuration;

    public class FilemanagerConfig
    {
        public FilemanagerConfig()
        {
            RootPath = WebConfigurationManager.AppSettings["FileManager_RootPath"];
            IconDirectory = WebConfigurationManager.AppSettings["Filemanager_IconDirectory"];
            AllowedExtensions = new List<string> { ".ai", ".asx", ".avi", ".bmp", ".csv", ".dat", ".doc", ".docx", ".epub", ".fla", ".flv", ".gif", ".html", ".ico", ".jpeg", ".jpg", ".m4a", ".mobi", ".mov", ".mp3", ".mp4", ".mpa", ".mpg", ".mpp", ".pdf", ".png", ".pps", ".ppsx", ".ppt", ".pptx", ".ps", ".psd", ".qt", ".ra", ".ram", ".rar", ".rm", ".rtf", ".svg", ".swf", ".tif", ".txt", ".vcf", ".vsd", ".wav", ".wks", ".wma", ".wmv", ".wps", ".xls", ".xlsx", ".xml", ".zip" };
            ImgExtensions = new List<string> { ".gif", ".jpe", ".jpeg", ".jpg", ".png", ".svg" };
        }

        public string RootPath { get; set; }

        public string IconDirectory { get; set; }

        public List<string> AllowedExtensions { get; set; }

        public List<string> ImgExtensions { get; set; }
    }
}