using Microsoft.AspNetCore.Mvc;
using prog_poe_st10249266.Models;
using System.Diagnostics;

namespace prog_poe_st10249266.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult viewClaims()
        {
            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public UserTBL usrtbl = new UserTBL();

        [HttpPost]
        public ActionResult SignUp(UserTBL Users)
        {
            var result = usrtbl.insert_User(Users);
            return RedirectToAction("Index", "Home");
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("userID");
            return RedirectToAction("Index", "Home");
        }


        [HttpGet]
        public ActionResult SignUp()
        {
            return View(usrtbl);
        }
        public ActionResult Login()
        {
            return View(usrtbl);
        }
    }
}