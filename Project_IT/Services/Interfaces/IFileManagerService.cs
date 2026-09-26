using System;
using System.Web;
using Project_IT.Models;

namespace Project_IT.Services.Interfaces
{
    public interface IFileManagerService
    {
        FileManagerViewModel GetFileManagerData(string currentPath, int page, string viewType, string query, Func<string, string> mapPath);
        bool UploadFile(HttpPostedFileBase file, string targetPath, Func<string, string> mapPath, out string errorMessage, out string sanitizedRelativePath);
        bool CreateFolder(string currentPath, string folderName, Func<string, string> mapPath, out string errorMessage, out string sanitizedRelativePath);
        bool DeleteItem(string itemPath, string currentPath, Func<string, string> mapPath, out string errorMessage);
        bool RenameItem(string itemPath, string newName, string currentPath, Func<string, string> mapPath, out string errorMessage);
    }
}
