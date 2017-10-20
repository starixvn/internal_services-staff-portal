namespace TicketDesk.Web.Client.Controllers
{
    using Core.Resources;
    using Criteria.FileManager;
    using Domain.Infrastructure;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Web.Attribute;
    using Service;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Web;
    using System.Web.Configuration;
    using System.Web.Mvc;


    [RoutePrefix("file-manager")]
    [Route("{action=index}")]
    public class FileManagerController : BaseController
    {
        private readonly string _webRootPath;
        private readonly string _webPath;
        private readonly List<string> _allowedExtensions;
        private readonly IFileManagerService _fileManagerService;

        public FileManagerController(IFileManagerService fileManagerService)
        {
            _webPath = WebConfigurationManager.AppSettings["FileManager_RootPath"];
            _webRootPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _webPath);
            _allowedExtensions = new List<string>
            {
                "jpg", "jpe", "jpeg", "gif", "png", "svg", "txt", "pdf", "odp", "ods", "odt", "rtf",
                "doc", "docx", "xls", "xlsx", "ppt", "pptx", "csv", "ogv", "avi", "mkv", "mp4", "webm", "m4v", "ogg", "mp3", "wav", "zip", "rar", "md"
            };

            if (!Directory.Exists(_webRootPath))
            {
                Directory.CreateDirectory(_webRootPath);
            }

            _fileManagerService = fileManagerService;
        }

        [NapAuth(FunctionKey = FunctionKeys.FileManagement, Policy = Constant.Policy.Read)]
        public ActionResult Index()
        {
            return View();
        }

        [ValidateInput(false)]
        [Route("filemanager")]
        [NapAuth(FunctionKey = FunctionKeys.FileManagement, Policy = Constant.Policy.Read)]
        public ActionResult FileManager(
            string mode,
            string path,
            string name,
            IEnumerable<HttpPostedFileBase> files,
            bool thumbnail = false)
        {
            string validPath = path;
            if (!string.IsNullOrWhiteSpace(path) && path.StartsWith("/"))
            {
                validPath = path.Substring(1);
            }
            
            var errorResult = new { Errors = new List<dynamic>() };

            switch (mode)
            {
                case "initiate":
                    return Json(Initiate(), JsonRequestBehavior.AllowGet);

                case "getfolder":
                    return Json(GetFolder(validPath), JsonRequestBehavior.AllowGet);

                case "addfolder":
                    return Json(AddFolder(validPath, name), JsonRequestBehavior.AllowGet);

                case "upload":
                    return Json(Upload(validPath, files), JsonRequestBehavior.AllowGet);

                case "delete":
                    return Json(Delete(validPath), JsonRequestBehavior.AllowGet);

                case "download":
                    if (Request.Headers["accept"].Contains("json"))
                    {
                        return Json(Download(validPath), JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        var file = DownloadFile(validPath);
                        return File(file.FileBytes, "application/x-msdownload", file.FileName);
                    }
                case "getimage":
                    return GetImage(validPath);

                case "readfile":
                    break;

                case "summarize":
                    return Json(Summarize(), JsonRequestBehavior.AllowGet);
                default:
                {
                    errorResult.Errors.Add(new
                    {
                        Code = "500",
                        Message = Messages.UnknownRequest,
                        Arguments = new List<string>
                        {
                            mode
                        }
                    });

                    break;
                }
            }

            return Json(errorResult, JsonRequestBehavior.AllowGet);
        }

        private static string MakeWebPath(string path, bool addSeperatorToBegin = false, bool addSeperatorToLast = false)
        {
            var validPath = path.Replace("\\", "/");

            if (addSeperatorToBegin)
            {
                validPath = "/" + validPath;
            }

            if (addSeperatorToLast)
            {
                validPath = validPath + "/";
            }

            return validPath;
        }

        private dynamic Initiate()
        {
            var result = new
            {
                Data = new
                {
                    Type = "initiate",
                    Attributes = new
                    {
                        Config = new
                        {
                            Security = new
                            {
                                ReadOnly = false,
                                Extensions = new
                                {
                                    IgnoreCase = true,
                                    Policy = "ALLOW_LIST",
                                    Restrictions = _allowedExtensions
                                }
                            }
                        }
                    }
                }
            };

            return result;
        }

        private dynamic GetFolder(string path)
        {
            string validPath = path;
            if(string.IsNullOrEmpty(validPath))
            {
                validPath = string.Empty;
            }

            var rootpath = Path.Combine(_webRootPath, validPath);
            var rootDirectory = new DirectoryInfo(rootpath);
            var data = new List<dynamic>();

            AddDirectoriesToFolder(validPath, rootDirectory, data);
            AddFilesToFolder(validPath, rootDirectory, data);

            var result = new { Data = data };

            return result;
        }

        private void AddFilesToFolder(string path, DirectoryInfo rootDirectory, List<dynamic> data)
        {
            foreach (var file in rootDirectory.GetFiles())
            {
                var item = new
                {
                    Id = MakeWebPath(Path.Combine(path, file.Name)),
                    Type = "file",
                    Attributes = new
                    {
                        file.Name,
                        Path = MakeWebPath(Path.Combine(_webPath, path, file.Name), true),
                        Readable = 1,
                        Writable = 1,
                        Created = file.CreationTime.ToString(CultureInfo.InvariantCulture),
                        Modified = file.LastWriteTime.ToString(CultureInfo.InvariantCulture),
                        Extension = file.Extension.Replace(".", string.Empty),
                        Size = file.Length,
                        Timestamp = (int)(DateTime.Now - file.LastWriteTime).Ticks
                    }
                };

                data.Add(item);
            }
        }

        private void AddDirectoriesToFolder(string path, DirectoryInfo rootDirectory, List<dynamic> data)
        {
            foreach (var directory in rootDirectory.GetDirectories())
            {
                var item = new
                {
                    Id = MakeWebPath(Path.Combine(path, directory.Name), addSeperatorToLast: true),
                    Type = "folder",
                    Attributes = new
                    {
                        directory.Name,
                        Path = MakeWebPath(Path.Combine(_webPath, path, directory.Name), true, true),
                        Readable = 1,
                        Writable = 1,
                        Created = directory.CreationTime.ToString(CultureInfo.InvariantCulture),
                        Modified = directory.LastWriteTime.ToString(CultureInfo.InvariantCulture),
                        Timestamp = (int)(DateTime.Now - directory.LastWriteTime).Ticks
                    }
                };

                data.Add(item);
            }
        }

        private dynamic AddFolder(string path, string name)
        {
            var invalidCharaters = new[] { "\\", "/", ":", "*", "?", "\"", "<", ">", "|" };
            if (invalidCharaters.Any(name.Contains))
            {
                var errorResult = new { Errors = new List<dynamic>() };

                errorResult.Errors.Add(new
                {
                    Code = "500",
                    Message = Messages.InvalidFolderName,
                    Arguments = new List<string>
                    {
                        name
                    }
                });

                return errorResult;
            }

            var newDirectoryPath = Path.Combine(_webRootPath, path, name);
            var directoryExist = Directory.Exists(newDirectoryPath);

            if (directoryExist)
            {
                var errorResult = new { Errors = new List<dynamic>() };

                errorResult.Errors.Add(new
                {
                    Code = "500",
                    Message = "DIRECTORY_ALREADY_EXISTS",
                    Arguments = new List<string>
                    {
                        name
                    }
                });

                return errorResult;
            }

            Directory.CreateDirectory(newDirectoryPath);
            var directory = new DirectoryInfo(newDirectoryPath);

            var result = new
            {
                Data =
                    new
                    {
                        Id = MakeWebPath(Path.Combine(path, directory.Name), addSeperatorToLast: true),
                        Type = "folder",
                        Attributes = new
                        {
                            directory.Name,
                            Path = MakeWebPath(Path.Combine(_webPath, path, directory.Name), true, true),
                            Readable = 1,
                            Writable = 1,
                            Created = DateTime.Now.ToString(CultureInfo.InvariantCulture),
                            Modified = DateTime.Now.ToString(CultureInfo.InvariantCulture)
                        }
                    }
            };

            var criteria = new AddFolderCriteria
            {
                UserId = CurrentSession.UserId,
                FolderName = directory.Name,
                Directory = MakeWebPath(Path.Combine(_webPath, path.EndsWith("/") ? path.Substring(0, path.LastIndexOf("/", StringComparison.Ordinal)) : path))
            };

            _fileManagerService.AddFolder(criteria);

            return result;
        }

        private dynamic Upload(string path, IEnumerable<HttpPostedFileBase> files)
        {
            var result = new
            {
                Data = new List<dynamic>()
            };

            foreach (var file in files)
            {
                if (file.ContentLength <= 0)
                {
                    continue;
                }

                var fileExist = System.IO.File.Exists(Path.Combine(_webRootPath, path, file.FileName));

                if (fileExist)
                {
                    var errorResult = new { Errors = new List<dynamic>() };

                    errorResult.Errors.Add(new
                    {
                        Code = "500",
                        Message = "FILE_ALREADY_EXISTS",
                        Arguments = new List<string>
                        {
                            file.FileName
                        }
                    });

                    return errorResult;
                }

                var filePath = Path.Combine(_webRootPath, path, file.FileName);
                file.SaveAs(filePath);

                result.Data.Add(new
                {
                    Id = MakeWebPath(Path.Combine(path, file.FileName)),
                    Type = "file",
                    Attributes = new
                    {
                        Name = file.FileName,
                        Extension = Path.GetExtension(file.FileName).Replace(".", string.Empty),
                        Path = MakeWebPath(Path.Combine(_webPath, path, file.FileName), true),
                        Readable = 1,
                        Writable = 1,
                        Created = DateTime.Now.ToString(CultureInfo.InvariantCulture),
                        Modified = DateTime.Now.ToString(CultureInfo.InvariantCulture),
                        Size = file.ContentLength
                    }
                });

                var criteria = new UploadFileCriteria
                {
                    UserId = CurrentSession.UserId,
                    FileName = file.FileName,
                    FileSize = file.ContentLength,
                    ContentType = file.ContentType,
                    Directory = MakeWebPath(Path.Combine(_webPath, path.EndsWith("/") ? path.Substring(0, path.LastIndexOf("/", StringComparison.Ordinal)) : path))
                };

                _fileManagerService.UploadFile(criteria);
            }

            return result;
        }

        private dynamic Delete(string path)
        {
            var fileAttributes = System.IO.File.GetAttributes(Path.Combine(_webRootPath, path));

            return fileAttributes == FileAttributes.Directory ? DeleteDirectory(path) : DeleteFile(path);
        }

        private dynamic DeleteDirectory(string path)
        {
            var webPath = MakeWebPath(Path.Combine(_webPath, path.EndsWith("/") ? path.Substring(0, path.LastIndexOf("/", StringComparison.Ordinal)) : path));
            var directoryName = Path.GetDirectoryName(path)?.Split('\\').Last();
            var rootpath = Path.Combine(_webRootPath, path);
            var rootDirectory = new DirectoryInfo(rootpath);

            if (rootDirectory.GetDirectories().Length > 0 || rootDirectory.GetFiles().Length > 0)
            {
                var errorResult = new { Errors = new List<dynamic>() };

                errorResult.Errors.Add(new
                {
                    Code = "500",
                    Message = "Can't delete the current folder because it contains the subfolders or a file list",
                    Arguments = new List<string>
                    {
                        directoryName
                    }
                });

                return errorResult;
            }

            Directory.Delete(Path.Combine(_webRootPath, path), true);

            var result = new
            {
                Data = new
                {
                    Id = path,
                    Type = "folder",
                    Attributes = new
                    {
                        Name = directoryName,
                        Readable = 1,
                        Writable = 1,
                        Modified = DateTime.Now.ToString(CultureInfo.InvariantCulture),
                        Path = path
                    }
                }
            };

            var criteria = new DeleteFolderCriteria
            {
                UserId = CurrentSession.UserId,
                FolderName = directoryName,
                Directory = webPath.Substring(0, webPath.LastIndexOf("/", StringComparison.Ordinal))
            };

            _fileManagerService.DeleteFolder(criteria);

            return result;
        }

        private dynamic DeleteFile(string path)
        {
            var webPath = MakeWebPath(Path.Combine(_webPath, path.EndsWith("/") ? path.Substring(0, path.LastIndexOf("/", StringComparison.Ordinal)) : path));
            var fileName = Path.GetFileName(Path.Combine(_webRootPath, path));
            var fileExtension = Path.GetExtension(fileName).Replace(".", string.Empty);
            System.IO.File.Delete(Path.Combine(_webRootPath, path));

            var result = new
            {
                Data = new
                {
                    Id = path,
                    Type = "file",
                    Attributes = new
                    {
                        Name = fileName,
                        Extension = fileExtension,
                        Readable = 1,
                        Writable = 1,
                        Modified = DateTime.Now.ToString(CultureInfo.InvariantCulture),
                    }
                }
            };

            var criteria = new DeleteFileCriteria
            {
                UserId = CurrentSession.UserId,
                FileName = fileName,
                Directory = webPath.Substring(0, webPath.LastIndexOf("/", StringComparison.Ordinal))
            };

            _fileManagerService.DeleteFile(criteria);

            return result;
        }

        private dynamic Download(string path)
        {
            var fileName = Path.GetFileName(Path.Combine(_webRootPath, path));
            var fileExtension = Path.GetExtension(fileName).Replace(".", string.Empty);

            var result = new
            {
                Data = new
                {
                    Id = path,
                    Type = "file",
                    Attributes = new
                    {
                        Name = fileName,
                        Extension = fileExtension,
                        Readable = 1,
                        Writable = 1,
                        Modified = DateTime.Now.ToString(CultureInfo.InvariantCulture),
                    }
                }
            };

            return result;
        }

        private dynamic DownloadFile(string path)
        {
            var filepath = Path.Combine(_webRootPath, path);
            var fileName = Path.GetFileName(filepath);
            byte[] fileBytes = System.IO.File.ReadAllBytes(filepath);

            var file = new
            {
                FileName = fileName,
                FileBytes = fileBytes
            };

            return file;
        }

        private ActionResult GetImage(string path)
        {
            var filepath = Path.Combine(_webRootPath, path);
            var fileName = Path.GetFileName(filepath);
            byte[] fileBytes = System.IO.File.ReadAllBytes(filepath);

            return File(fileBytes, "application/x-msdownload", fileName);
        }

        private dynamic Summarize()
        {
            var directories = Directory.GetDirectories(_webRootPath, "*", SearchOption.AllDirectories).Length;

            var directoryInfo = new DirectoryInfo(_webRootPath);
            var files = directoryInfo.GetFiles("*", SearchOption.AllDirectories);
            var allSize = files.Select(f => f.Length).Sum();

            var result = new
            {
                Data = new
                {
                    Id = "/",
                    Type = "summary",
                    Attributes = new
                    {
                        Size = allSize,
                        Files = files.Length,
                        Folders = directories,
                        SizeLimit = 0
                    }
                }
            };

            return result;
        }
    }
}