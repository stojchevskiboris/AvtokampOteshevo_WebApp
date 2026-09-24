using System;
using System.Web;
using System.Web.Mvc;
using log4net;
using Project_IT.Services.Interfaces;

namespace Project_IT.Controllers
{
    [Authorize]
    public class AdminFileManagerController : Controller
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(AdminFileManagerController));
        private readonly IFileManagerService _fileManagerService;

        public AdminFileManagerController(IFileManagerService fileManagerService)
        {
            _fileManagerService = fileManagerService;
        }

        // GET: AdminFileManager
        public ActionResult Index(string currentPath = "Content", int page = 1, string viewType = "grid", string query = null)
        {
            try
            {
                var model = _fileManagerService.GetFileManagerData(currentPath, page, viewType, query, Server.MapPath);
                return View(model);
            }
            catch (Exception ex)
            {
                log.Error("Error in Index: " + ex.Message, ex);
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Upload(HttpPostedFileBase file, string targetPath)
        {
            try
            {
                bool success = _fileManagerService.UploadFile(file, targetPath, Server.MapPath, out string errorMessage, out string sanitizedRelativePath);
                if (success)
                {
                    TempData["SuccessMessage"] = "Фајлот е успешно прикачен.";
                    return RedirectToRouteOrPath(sanitizedRelativePath);
                }

                TempData["ErrorMessage"] = errorMessage;
                return RedirectToRouteOrPath(sanitizedRelativePath);
            }
            catch (Exception ex)
            {
                log.Error("Error in Upload: " + ex.Message, ex);
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateFolder(string currentPath, string folderName)
        {
            try
            {
                bool success = _fileManagerService.CreateFolder(currentPath, folderName, Server.MapPath, out string errorMessage, out string sanitizedRelativePath);
                if (success)
                {
                    TempData["SuccessMessage"] = "Фолдерот е успешно креиран.";
                    return RedirectToRouteOrPath(sanitizedRelativePath);
                }

                TempData["ErrorMessage"] = errorMessage;
                return RedirectToRouteOrPath(sanitizedRelativePath);
            }
            catch (Exception ex)
            {
                log.Error("Error in CreateFolder: " + ex.Message, ex);
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(string itemPath, string currentPath = "Content")
        {
            try
            {
                bool success = _fileManagerService.DeleteItem(itemPath, currentPath, Server.MapPath, out string errorMessage);
                if (success)
                {
                    TempData["SuccessMessage"] = "Елементот е успешно избришан.";
                }
                else
                {
                    TempData["ErrorMessage"] = errorMessage;
                }

                return RedirectToRouteOrPath(currentPath);
            }
            catch (Exception ex)
            {
                log.Error("Error in Delete: " + ex.Message, ex);
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Rename(string itemPath, string newName, string currentPath = "Content")
        {
            try
            {
                bool success = _fileManagerService.RenameItem(itemPath, newName, currentPath, Server.MapPath, out string errorMessage);
                if (success)
                {
                    TempData["SuccessMessage"] = "Елементот е успешно преименуван.";
                }
                else
                {
                    TempData["ErrorMessage"] = errorMessage;
                }

                return RedirectToRouteOrPath(currentPath);
            }
            catch (Exception ex)
            {
                log.Error("Error in Rename: " + ex.Message, ex);
                return RedirectToAction("Error", "Home");
            }
        }

        #region Helper Methods

        private ActionResult RedirectToRouteOrPath(string currentPath)
        {
            return RedirectToAction("Index", new { currentPath = string.IsNullOrWhiteSpace(currentPath) ? "Content" : "Content/" + currentPath });
        }

        #endregion
    }
}
