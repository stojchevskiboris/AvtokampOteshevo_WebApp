using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using log4net;
using Project_IT.Models;

namespace Project_IT.Controllers
{
    [Authorize]
    public class ManageController : Controller
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ManageController));

        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public ManageController()
        {
        }

        public ManageController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //
        // GET: /Manage/Index
        public async Task<ActionResult> Index(ManageMessageId? message)
        {
            try
            {
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                log.Error("Error in Index: " + ex.Message, ex);
                return RedirectToAction("Error", "Home");
            }
        }

        //
        // POST: /Manage/RemoveLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemoveLogin(string loginProvider, string providerKey)
        {
            try
            {
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                log.Error("Error in RemoveLogin: " + ex.Message, ex);
                return RedirectToAction("Error", "Home");
            }
        }

        //
        // GET: /Manage/AddPhoneNumber
        public ActionResult AddPhoneNumber()
        {
            try
            {
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                log.Error("Error in AddPhoneNumber: " + ex.Message, ex);
                return RedirectToAction("Error", "Home");
            }
        }

        //
        // POST: /Manage/AddPhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddPhoneNumber(AddPhoneNumberViewModel model)
        {
            try
            {
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                log.Error("Error in AddPhoneNumber [POST]: " + ex.Message, ex);
                return RedirectToAction("Error", "Home");
            }
        }

        //
        // POST: /Manage/EnableTwoFactorAuthentication
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EnableTwoFactorAuthentication()
        {
            try
            {
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                log.Error("Error in EnableTwoFactorAuthentication: " + ex.Message, ex);
                return RedirectToAction("Error", "Home");
            }
        }

        //
        // POST: /Manage/DisableTwoFactorAuthentication
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DisableTwoFactorAuthentication()
        {
            try
            {
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                log.Error("Error in DisableTwoFactorAuthentication: " + ex.Message, ex);
                return RedirectToAction("Error", "Home");
            }
        }

        //
        // GET: /Manage/VerifyPhoneNumber
        public async Task<ActionResult> VerifyPhoneNumber(string phoneNumber)
        {
            try
            {
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                log.Error("Error in VerifyPhoneNumber: " + ex.Message, ex);
                return RedirectToAction("Error", "Home");
            }
        }

        //
        // POST: /Manage/VerifyPhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyPhoneNumber(VerifyPhoneNumberViewModel model)
        {
            try
            {
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                log.Error("Error in VerifyPhoneNumber [POST]: " + ex.Message, ex);
                return RedirectToAction("Error", "Home");
            }
        }

        //
        // POST: /Manage/RemovePhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemovePhoneNumber()
        {
            try
            {
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                log.Error("Error in RemovePhoneNumber: " + ex.Message, ex);
                return RedirectToAction("Error", "Home");
            }
        }

        //
        // GET: /Manage/ChangePassword
        public ActionResult ChangePassword()
        {
            try
            {
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                log.Error("Error in ChangePassword: " + ex.Message, ex);
                return RedirectToAction("Error", "Home");
            }
        }

        //
        // POST: /Manage/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            try
            {
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                log.Error("Error in ChangePassword [POST]: " + ex.Message, ex);
                return RedirectToAction("Error", "Home");
            }
        }

        //
        // GET: /Manage/SetPassword
        public ActionResult SetPassword()
        {
            try
            {
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                log.Error("Error in SetPassword: " + ex.Message, ex);
                return RedirectToAction("Error", "Home");
            }
        }

        //
        // POST: /Manage/SetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SetPassword(SetPasswordViewModel model)
        {
            try
            {
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                log.Error("Error in SetPassword [POST]: " + ex.Message, ex);
                return RedirectToAction("Error", "Home");
            }
        }

        //
        // GET: /Manage/ManageLogins
        public async Task<ActionResult> ManageLogins(ManageMessageId? message)
        {
            try
            {
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                log.Error("Error in ManageLogins: " + ex.Message, ex);
                return RedirectToAction("Error", "Home");
            }
        }

        //
        // POST: /Manage/LinkLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LinkLogin(string provider)
        {
            try
            {
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                log.Error("Error in LinkLogin: " + ex.Message, ex);
                return RedirectToAction("Error", "Home");
            }
        }

        //
        // GET: /Manage/LinkLoginCallback
        public async Task<ActionResult> LinkLoginCallback()
        {
            try
            {
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                log.Error("Error in LinkLoginCallback: " + ex.Message, ex);
                return RedirectToAction("Error", "Home");
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _userManager != null)
            {
                _userManager.Dispose();
                _userManager = null;
            }

            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private bool HasPassword()
        {
            if (User != null && User.Identity != null)
            {
                var user = UserManager.FindById(User.Identity.GetUserId());
                if (user != null)
                {
                    return user.PasswordHash != null;
                }
            }
            return false;
        }

        private bool HasPhoneNumber()
        {
            if (User != null && User.Identity != null)
            {
                var user = UserManager.FindById(User.Identity.GetUserId());
                if (user != null)
                {
                    return user.PhoneNumber != null;
                }
            }
            return false;
        }

        public enum ManageMessageId
        {
            AddPhoneSuccess,
            ChangePasswordSuccess,
            SetTwoFactorSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            RemovePhoneSuccess,
            Error
        }

        #endregion
    }
}
