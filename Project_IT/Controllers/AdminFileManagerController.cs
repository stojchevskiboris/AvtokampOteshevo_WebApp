using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Project_IT.Models;

namespace Project_IT.Controllers
{
    [Authorize]
    public class AdminFileManagerController : Controller
    {
        private const int PageSize = 30;
        private static readonly HashSet<string> AllowedUploadExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            ".jpg", ".jpeg", ".png", ".gif", ".webp", ".svg",
            ".pdf",
            ".css", ".js", ".txt"
        };

        private static readonly HashSet<string> ImageExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            ".jpg", ".jpeg", ".png", ".gif", ".webp", ".svg"
        };

        // GET: AdminFileManager
        public ActionResult Index(string currentPath = "Content", int page = 1, string viewType = "grid", string query = null)
        {
            if (page < 1) page = 1;
            viewType = string.Equals(viewType, "list", StringComparison.OrdinalIgnoreCase) ? "list" : "grid";

            string rootPhysicalPath = GetNormalizedPath(Server.MapPath("~/Content/"));
            string relativeCleanPath = SanitizeRelativePath(currentPath);
            string targetPhysicalPath = GetNormalizedPath(Path.Combine(rootPhysicalPath, relativeCleanPath));

            if (!IsPathWithinRoot(targetPhysicalPath, rootPhysicalPath))
            {
                TempData["ErrorMessage"] = "Невалидна патека!";
                return RedirectToAction("Index");
            }

            if (!Directory.Exists(targetPhysicalPath))
            {
                // Fallback to root Content if target directory does not exist
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

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Upload(HttpPostedFileBase file, string targetPath)
        {
            if (file == null || file.ContentLength == 0)
            {
                TempData["ErrorMessage"] = "Ве молиме изберете фајл за прикачување.";
                return RedirectToRouteOrPath(targetPath);
            }

            string ext = Path.GetExtension(file.FileName);
            if (string.IsNullOrEmpty(ext) || !AllowedUploadExtensions.Contains(ext))
            {
                TempData["ErrorMessage"] = "Форматот на фајлот не е дозволен.";
                return RedirectToRouteOrPath(targetPath);
            }

            string rootPhysicalPath = GetNormalizedPath(Server.MapPath("~/Content/"));
            string relativeCleanPath = SanitizeRelativePath(targetPath);
            string targetDirPhysicalPath = GetNormalizedPath(Path.Combine(rootPhysicalPath, relativeCleanPath));

            if (!IsPathWithinRoot(targetDirPhysicalPath, rootPhysicalPath))
            {
                TempData["ErrorMessage"] = "Невалидна патека!";
                return RedirectToAction("Index");
            }

            if (!Directory.Exists(targetDirPhysicalPath))
            {
                Directory.CreateDirectory(targetDirPhysicalPath);
            }

            string fileName = Path.GetFileName(file.FileName);
            string savePath = Path.Combine(targetDirPhysicalPath, fileName);

            if (!IsPathWithinRoot(savePath, rootPhysicalPath))
            {
                TempData["ErrorMessage"] = "Невалидно име на фајл!";
                return RedirectToAction("Index");
            }

            file.SaveAs(savePath);
            TempData["SuccessMessage"] = "Фајлот е успешно прикачен.";

            return RedirectToRouteOrPath(relativeCleanPath);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateFolder(string currentPath, string folderName)
        {
            if (string.IsNullOrWhiteSpace(folderName))
            {
                TempData["ErrorMessage"] = "Името на фолдерот е задолжително.";
                return RedirectToRouteOrPath(currentPath);
            }

            string sanitizedFolderName = SanitizeFileName(folderName);
            if (string.IsNullOrWhiteSpace(sanitizedFolderName))
            {
                TempData["ErrorMessage"] = "Невалидно име на фолдер.";
                return RedirectToRouteOrPath(currentPath);
            }

            string rootPhysicalPath = GetNormalizedPath(Server.MapPath("~/Content/"));
            string relativeCleanPath = SanitizeRelativePath(currentPath);
            string targetDirPhysicalPath = GetNormalizedPath(Path.Combine(rootPhysicalPath, relativeCleanPath));

            if (!IsPathWithinRoot(targetDirPhysicalPath, rootPhysicalPath))
            {
                TempData["ErrorMessage"] = "Невалидна патека!";
                return RedirectToAction("Index");
            }

            string newFolderPath = Path.Combine(targetDirPhysicalPath, sanitizedFolderName);
            if (!IsPathWithinRoot(newFolderPath, rootPhysicalPath))
            {
                TempData["ErrorMessage"] = "Невалидна патека за фолдер!";
                return RedirectToAction("Index");
            }

            if (!Directory.Exists(newFolderPath))
            {
                Directory.CreateDirectory(newFolderPath);
                TempData["SuccessMessage"] = "Фолдерот е успешно креиран.";
            }
            else
            {
                TempData["ErrorMessage"] = "Фолдер со тоа име веќе постои.";
            }

            return RedirectToRouteOrPath(relativeCleanPath);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(string itemPath, string currentPath = "Content")
        {
            if (string.IsNullOrWhiteSpace(itemPath))
            {
                TempData["ErrorMessage"] = "Невалидна патека.";
                return RedirectToRouteOrPath(currentPath);
            }

            string rootPhysicalPath = GetNormalizedPath(Server.MapPath("~/Content/"));
            string relativeCleanPath = SanitizeRelativePath(itemPath);
            string physicalPath = GetNormalizedPath(Path.Combine(rootPhysicalPath, relativeCleanPath));

            if (!IsPathWithinRoot(physicalPath, rootPhysicalPath) || physicalPath.Equals(rootPhysicalPath, StringComparison.OrdinalIgnoreCase))
            {
                TempData["ErrorMessage"] = "Не можете да го избришете коренот на Content!";
                return RedirectToAction("Index");
            }

            try
            {
                if (Directory.Exists(physicalPath))
                {
                    Directory.Delete(physicalPath, true);
                    TempData["SuccessMessage"] = "Фолдерот е успешно избришан.";
                }
                else if (File.Exists(physicalPath))
                {
                    System.IO.File.Delete(physicalPath);
                    TempData["SuccessMessage"] = "Фајлот е успешно избришан.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Елементот не постои.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Грешка при бришење: " + ex.Message;
            }

            return RedirectToRouteOrPath(currentPath);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Rename(string itemPath, string newName, string currentPath = "Content")
        {
            if (string.IsNullOrWhiteSpace(itemPath) || string.IsNullOrWhiteSpace(newName))
            {
                TempData["ErrorMessage"] = "Невалидни параметри за преименување.";
                return RedirectToRouteOrPath(currentPath);
            }

            string sanitizedNewName = SanitizeFileName(newName);
            if (string.IsNullOrWhiteSpace(sanitizedNewName))
            {
                TempData["ErrorMessage"] = "Невалидно ново име.";
                return RedirectToRouteOrPath(currentPath);
            }

            string rootPhysicalPath = GetNormalizedPath(Server.MapPath("~/Content/"));
            string relativeCleanPath = SanitizeRelativePath(itemPath);
            string sourcePhysicalPath = GetNormalizedPath(Path.Combine(rootPhysicalPath, relativeCleanPath));

            if (!IsPathWithinRoot(sourcePhysicalPath, rootPhysicalPath) || sourcePhysicalPath.Equals(rootPhysicalPath, StringComparison.OrdinalIgnoreCase))
            {
                TempData["ErrorMessage"] = "Невалидна патека за преименување.";
                return RedirectToAction("Index");
            }

            string parentDirectory = Path.GetDirectoryName(sourcePhysicalPath);
            string destinationPhysicalPath = Path.Combine(parentDirectory, sanitizedNewName);

            if (!IsPathWithinRoot(destinationPhysicalPath, rootPhysicalPath))
            {
                TempData["ErrorMessage"] = "Невалидна дестинација за преименување.";
                return RedirectToAction("Index");
            }

            try
            {
                if (Directory.Exists(sourcePhysicalPath))
                {
                    if (Directory.Exists(destinationPhysicalPath))
                    {
                        TempData["ErrorMessage"] = "Фолдер со исто име веќе постои.";
                    }
                    else
                    {
                        Directory.Move(sourcePhysicalPath, destinationPhysicalPath);
                        TempData["SuccessMessage"] = "Фолдерот е успешно преименуван.";
                    }
                }
                else if (File.Exists(sourcePhysicalPath))
                {
                    if (File.Exists(destinationPhysicalPath))
                    {
                        TempData["ErrorMessage"] = "Фајл со исто име веќе постои.";
                    }
                    else
                    {
                        System.IO.File.Move(sourcePhysicalPath, destinationPhysicalPath);
                        TempData["SuccessMessage"] = "Фајлот е успешно преименуван.";
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "Елементот не постои.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Грешка при преименување: " + ex.Message;
            }

            return RedirectToRouteOrPath(currentPath);
        }

        #region Helper Methods

        private ActionResult RedirectToRouteOrPath(string currentPath)
        {
            string clean = SanitizeRelativePath(currentPath);
            return RedirectToAction("Index", new { currentPath = string.IsNullOrWhiteSpace(clean) ? "Content" : "Content/" + clean });
        }

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
