using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using log4net;
using Project_IT.Models;
using Project_IT.Services.Interfaces;

namespace Project_IT.Services.Implementations
{
    public class FileManagerService : IFileManagerService
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(FileManagerService));
        private const int PageSize = 30;

        private static readonly HashSet<string> AllowedUploadExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            ".jpg", ".jpeg", ".png", ".gif", ".webp", ".svg",
            ".mp4", ".webm", ".ogg",
            ".pdf",
            ".css", ".js", ".txt"
        };

        private static readonly HashSet<string> ImageExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            ".jpg", ".jpeg", ".png", ".gif", ".webp", ".svg"
        };

        public FileManagerViewModel GetFileManagerData(string currentPath, int page, string viewType, string query, Func<string, string> mapPath)
        {
            try
            {
                if (page < 1) page = 1;
                viewType = string.Equals(viewType, "list", StringComparison.OrdinalIgnoreCase) ? "list" : "grid";

                string rootPhysicalPath = GetNormalizedPath(mapPath("~/Content/"));
                string relativeCleanPath = SanitizeRelativePath(currentPath);
                string targetPhysicalPath = GetNormalizedPath(Path.Combine(rootPhysicalPath, relativeCleanPath));

                if (!IsPathWithinRoot(targetPhysicalPath, rootPhysicalPath))
                {
                    relativeCleanPath = "";
                    targetPhysicalPath = rootPhysicalPath;
                }

                if (!Directory.Exists(targetPhysicalPath))
                {
                    relativeCleanPath = "";
                    targetPhysicalPath = rootPhysicalPath;
                }

                var model = new FileManagerViewModel
                {
                    CurrentPath = string.IsNullOrWhiteSpace(relativeCleanPath) ? "Content" : "Content/" + relativeCleanPath,
                    SearchQuery = query?.Trim(),
                    ViewType = viewType,
                    CurrentPage = page,
                    Breadcrumbs = BuildBreadcrumbs(relativeCleanPath)
                };

                List<FileItemViewModel> allItems = new List<FileItemViewModel>();

                if (!string.IsNullOrWhiteSpace(model.SearchQuery))
                {
                    string searchPattern = model.SearchQuery.ToLower();
                    DirectoryInfo rootDir = new DirectoryInfo(rootPhysicalPath);

                    SearchFilesRecursive(rootDir, rootPhysicalPath, searchPattern, allItems);
                }
                else
                {
                    DirectoryInfo dir = new DirectoryInfo(targetPhysicalPath);

                    var dirEntries = dir.GetDirectories()
                        .OrderBy(d => d.Name)
                        .Select(d => MapToViewModel(d, rootPhysicalPath))
                        .ToList();

                    var fileEntries = dir.GetFiles()
                        .OrderBy(f => f.Name)
                        .Select(f => MapToViewModel(f, rootPhysicalPath))
                        .ToList();

                    allItems.AddRange(dirEntries);
                    allItems.AddRange(fileEntries);
                }

                model.TotalItems = allItems.Count;
                model.TotalPages = (int)Math.Ceiling((double)allItems.Count / PageSize);
                if (model.TotalPages < 1) model.TotalPages = 1;

                if (model.CurrentPage > model.TotalPages)
                {
                    model.CurrentPage = model.TotalPages;
                }

                model.Items = allItems
                    .Skip((model.CurrentPage - 1) * PageSize)
                    .Take(PageSize)
                    .ToList();

                return model;
            }
            catch (Exception ex)
            {
                log.Error("Error getting file manager data: " + ex.Message, ex);
                throw;
            }
        }

        public bool UploadFile(HttpPostedFileBase file, string targetPath, Func<string, string> mapPath, out string errorMessage, out string sanitizedRelativePath)
        {
            errorMessage = null;
            sanitizedRelativePath = SanitizeRelativePath(targetPath);

            try
            {
                if (file == null || file.ContentLength == 0)
                {
                    errorMessage = "Ве молиме изберете фајл за прикачување.";
                    return false;
                }

                string ext = Path.GetExtension(file.FileName);
                if (string.IsNullOrEmpty(ext) || !AllowedUploadExtensions.Contains(ext))
                {
                    errorMessage = "Форматот на фајлот не е дозволен.";
                    return false;
                }

                string rootPhysicalPath = GetNormalizedPath(mapPath("~/Content/"));
                string targetDirPhysicalPath = GetNormalizedPath(Path.Combine(rootPhysicalPath, sanitizedRelativePath));

                if (!IsPathWithinRoot(targetDirPhysicalPath, rootPhysicalPath))
                {
                    errorMessage = "Невалидна патека!";
                    return false;
                }

                if (!Directory.Exists(targetDirPhysicalPath))
                {
                    Directory.CreateDirectory(targetDirPhysicalPath);
                }

                string fileName = Path.GetFileName(file.FileName);
                string savePath = Path.Combine(targetDirPhysicalPath, fileName);

                if (!IsPathWithinRoot(savePath, rootPhysicalPath))
                {
                    errorMessage = "Невалидно име на фајл!";
                    return false;
                }

                file.SaveAs(savePath);
                return true;
            }
            catch (Exception ex)
            {
                log.Error("Error uploading file: " + ex.Message, ex);
                throw;
            }
        }

        public bool CreateFolder(string currentPath, string folderName, Func<string, string> mapPath, out string errorMessage, out string sanitizedRelativePath)
        {
            errorMessage = null;
            sanitizedRelativePath = SanitizeRelativePath(currentPath);

            try
            {
                if (string.IsNullOrWhiteSpace(folderName))
                {
                    errorMessage = "Името на фолдерот е задолжително.";
                    return false;
                }

                string sanitizedFolderName = SanitizeFileName(folderName);
                if (string.IsNullOrWhiteSpace(sanitizedFolderName))
                {
                    errorMessage = "Невалидно име на фолдер.";
                    return false;
                }

                string rootPhysicalPath = GetNormalizedPath(mapPath("~/Content/"));
                string targetDirPhysicalPath = GetNormalizedPath(Path.Combine(rootPhysicalPath, sanitizedRelativePath));

                if (!IsPathWithinRoot(targetDirPhysicalPath, rootPhysicalPath))
                {
                    errorMessage = "Невалидна патека!";
                    return false;
                }

                string newFolderPath = Path.Combine(targetDirPhysicalPath, sanitizedFolderName);
                if (!IsPathWithinRoot(newFolderPath, rootPhysicalPath))
                {
                    errorMessage = "Невалидна патека за фолдер!";
                    return false;
                }

                if (!Directory.Exists(newFolderPath))
                {
                    Directory.CreateDirectory(newFolderPath);
                    return true;
                }
                else
                {
                    errorMessage = "Фолдер со тоа име веќе постои.";
                    return false;
                }
            }
            catch (Exception ex)
            {
                log.Error("Error creating folder: " + ex.Message, ex);
                throw;
            }
        }

        public bool DeleteItem(string itemPath, string currentPath, Func<string, string> mapPath, out string errorMessage)
        {
            errorMessage = null;

            try
            {
                if (string.IsNullOrWhiteSpace(itemPath))
                {
                    errorMessage = "Невалидна патека.";
                    return false;
                }

                string rootPhysicalPath = GetNormalizedPath(mapPath("~/Content/"));
                string relativeCleanPath = SanitizeRelativePath(itemPath);
                string physicalPath = GetNormalizedPath(Path.Combine(rootPhysicalPath, relativeCleanPath));

                if (!IsPathWithinRoot(physicalPath, rootPhysicalPath) || physicalPath.Equals(rootPhysicalPath, StringComparison.OrdinalIgnoreCase))
                {
                    errorMessage = "Не можете да го избришете коренот на Content!";
                    return false;
                }

                if (Directory.Exists(physicalPath))
                {
                    Directory.Delete(physicalPath, true);
                    return true;
                }
                else if (File.Exists(physicalPath))
                {
                    File.Delete(physicalPath);
                    return true;
                }
                else
                {
                    errorMessage = "Елементот не постои.";
                    return false;
                }
            }
            catch (Exception ex)
            {
                log.Error("Error deleting item: " + ex.Message, ex);
                throw;
            }
        }

        public bool RenameItem(string itemPath, string newName, string currentPath, Func<string, string> mapPath, out string errorMessage)
        {
            errorMessage = null;

            try
            {
                if (string.IsNullOrWhiteSpace(itemPath) || string.IsNullOrWhiteSpace(newName))
                {
                    errorMessage = "Невалидни параметри за преименување.";
                    return false;
                }

                string sanitizedNewName = SanitizeFileName(newName);
                if (string.IsNullOrWhiteSpace(sanitizedNewName))
                {
                    errorMessage = "Невалидно ново име.";
                    return false;
                }

                string rootPhysicalPath = GetNormalizedPath(mapPath("~/Content/"));
                string relativeCleanPath = SanitizeRelativePath(itemPath);
                string sourcePhysicalPath = GetNormalizedPath(Path.Combine(rootPhysicalPath, relativeCleanPath));

                if (!IsPathWithinRoot(sourcePhysicalPath, rootPhysicalPath) || sourcePhysicalPath.Equals(rootPhysicalPath, StringComparison.OrdinalIgnoreCase))
                {
                    errorMessage = "Невалидна патека за преименување.";
                    return false;
                }

                string parentDirectory = Path.GetDirectoryName(sourcePhysicalPath);
                string destinationPhysicalPath = Path.Combine(parentDirectory, sanitizedNewName);

                if (!IsPathWithinRoot(destinationPhysicalPath, rootPhysicalPath))
                {
                    errorMessage = "Невалидна дестинација за преименување.";
                    return false;
                }

                if (Directory.Exists(sourcePhysicalPath))
                {
                    if (Directory.Exists(destinationPhysicalPath))
                    {
                        errorMessage = "Фолдер со исто име веќе постои.";
                        return false;
                    }
                    Directory.Move(sourcePhysicalPath, destinationPhysicalPath);
                    return true;
                }
                else if (File.Exists(sourcePhysicalPath))
                {
                    if (File.Exists(destinationPhysicalPath))
                    {
                        errorMessage = "Фајл со исто име веќе постои.";
                        return false;
                    }
                    File.Move(sourcePhysicalPath, destinationPhysicalPath);
                    return true;
                }
                else
                {
                    errorMessage = "Елементот не постои.";
                    return false;
                }
            }
            catch (Exception ex)
            {
                log.Error("Error renaming item: " + ex.Message, ex);
                throw;
            }
        }

        #region Helper Methods

        private static string GetNormalizedPath(string path)
        {
            if (string.IsNullOrEmpty(path)) return string.Empty;
            return Path.GetFullPath(path).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        }

        private static string SanitizeRelativePath(string rawPath)
        {
            if (string.IsNullOrWhiteSpace(rawPath)) return string.Empty;

            string path = rawPath.Trim();
            if (path.StartsWith("Content", StringComparison.OrdinalIgnoreCase))
            {
                path = path.Substring(7);
            }
            path = path.TrimStart('/', '\\');
            path = path.Replace('/', Path.DirectorySeparatorChar).Replace('\\', Path.DirectorySeparatorChar);

            return path;
        }

        private static string SanitizeFileName(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName)) return string.Empty;
            char[] invalidChars = Path.GetInvalidFileNameChars();
            return string.Concat(fileName.Split(invalidChars)).Trim();
        }

        private static bool IsPathWithinRoot(string targetPath, string rootPath)
        {
            string normTarget = GetNormalizedPath(targetPath);
            string normRoot = GetNormalizedPath(rootPath);

            return normTarget.Equals(normRoot, StringComparison.OrdinalIgnoreCase) ||
                   normTarget.StartsWith(normRoot + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase);
        }

        private static List<BreadcrumbItem> BuildBreadcrumbs(string relativeCleanPath)
        {
            var breadcrumbs = new List<BreadcrumbItem>
            {
                new BreadcrumbItem { Name = "Content", Path = "Content" }
            };

            if (string.IsNullOrWhiteSpace(relativeCleanPath))
                return breadcrumbs;

            string[] parts = relativeCleanPath.Split(new[] { Path.DirectorySeparatorChar, '/' }, StringSplitOptions.RemoveEmptyEntries);
            string accumulated = "Content";

            foreach (var part in parts)
            {
                accumulated += "/" + part;
                breadcrumbs.Add(new BreadcrumbItem
                {
                    Name = part,
                    Path = accumulated
                });
            }

            return breadcrumbs;
        }

        private void SearchFilesRecursive(DirectoryInfo dir, string rootPath, string searchPattern, List<FileItemViewModel> results)
        {
            try
            {
                foreach (var file in dir.GetFiles())
                {
                    if (file.Name.ToLower().Contains(searchPattern))
                    {
                        results.Add(MapToViewModel(file, rootPath));
                    }
                }

                foreach (var subDir in dir.GetDirectories())
                {
                    SearchFilesRecursive(subDir, rootPath, searchPattern, results);
                }
            }
            catch
            {
                // Ignore inaccessible directories/files gracefully
            }
        }

        private FileItemViewModel MapToViewModel(DirectoryInfo d, string rootPath)
        {
            string relFromRoot = d.FullName.Substring(rootPath.Length).TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            string relUrlPath = string.IsNullOrWhiteSpace(relFromRoot) ? "Content" : "Content/" + relFromRoot.Replace('\\', '/');

            return new FileItemViewModel
            {
                Name = d.Name,
                VirtualPath = "/" + relUrlPath,
                RelativePath = relUrlPath,
                IsDirectory = true,
                SizeBytes = 0,
                FormattedSize = "-",
                CreatedOn = d.CreationTime,
                ModifiedOn = d.LastWriteTime,
                Extension = "",
                IsImage = false
            };
        }

        private FileItemViewModel MapToViewModel(FileInfo f, string rootPath)
        {
            string relFromRoot = f.FullName.Substring(rootPath.Length).TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            string relUrlPath = "Content/" + relFromRoot.Replace('\\', '/');
            string ext = f.Extension.ToLower();
            bool isImg = ImageExtensions.Contains(ext);

            int? width = null;
            int? height = null;

            if (isImg && !ext.Equals(".svg", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    using (var img = Image.FromFile(f.FullName))
                    {
                        width = img.Width;
                        height = img.Height;
                    }
                }
                catch
                {
                    // If image reading fails, keep dimensions null
                }
            }

            return new FileItemViewModel
            {
                Name = f.Name,
                VirtualPath = "/" + relUrlPath,
                RelativePath = relUrlPath,
                IsDirectory = false,
                SizeBytes = f.Length,
                FormattedSize = FormatBytes(f.Length),
                CreatedOn = f.CreationTime,
                ModifiedOn = f.LastWriteTime,
                Extension = ext,
                IsImage = isImg,
                ImageWidth = width,
                ImageHeight = height
            };
        }

        private static string FormatBytes(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            double len = bytes;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len /= 1024;
            }
            return $"{len:0.##} {sizes[order]}";
        }

        #endregion
    }
}
