using prog_poe_st10249266.Models;
using Microsoft.AspNetCore.Mvc;

namespace prog_poe_st10249266.Controllers
{
    public class LoginController : Controller
    {
        private readonly LoginModel login;

        public LoginController()
        {
            login = new LoginModel();
        }

        [HttpPost]
        //    public ActionResult Login(string email, string password)
        //    {
        //        var loginModel = new LoginModel();
        //        int userID = loginModel.SelectUser(email, password);
        //        if (userID != -1)
        //        {
        //            HttpContext.Session.SetInt32("userID", userID);
        //            return RedirectToAction("Index", "Home", new { userID = userID });
        //        }
        //        else
        //        {
        //            TempData["AlertMessage"] = "Username or password is incorrect!";
        //            return RedirectToAction("Login", "User");
        //        }
        //    }
        //}
        [HttpPost]
        public ActionResult Login(string email, string password)
        {
            var loginModel = new LoginModel();
            var (userID, isAdmin) = loginModel.SelectUser(email, password);
            if (userID != -1)
            {
                HttpContext.Session.SetInt32("userID", userID);
                HttpContext.Session.SetInt32("isAdmin", isAdmin); // Store isAdmin as an int
                return RedirectToAction("Index", "Home", new { userID = userID });
            }
            else
            {
                TempData["AlertMessage"] = "Username or password is incorrect!";
                return RedirectToAction("Login", "User");
            }
        }
    }
}