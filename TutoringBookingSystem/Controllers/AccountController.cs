using System.Linq;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Security;
using TutoringBookingSystem.Models;

namespace TutoringBookingSystem.Controllers
{
    public class AccountController : Controller
    {
        private TutoringContext db = new TutoringContext();

        // GET: Account/Login
        public ActionResult Login()
        {
            return View();
        }

        // POST: Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string username, string password)
        {
            var tutor = db.Tutors.FirstOrDefault(t => t.Username == username);

            if (tutor != null && Crypto.VerifyHashedPassword(tutor.PasswordHash, password))
            {
                FormsAuthentication.SetAuthCookie(tutor.Username, false);
                return RedirectToAction("Index", "Bookings");
            }

            ModelState.AddModelError("", "Invalid username or password.");
            return View();
        }

        // GET: Account/Logout
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}