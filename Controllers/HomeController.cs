using Microsoft.AspNetCore.Mvc;
using prog_poe_st10249266.Models;
using System.Diagnostics;
using System.Security.Claims;

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
            int? userID = HttpContext.Session.GetInt32("userID");
            int? isAdmin = HttpContext.Session.GetInt32("isAdmin");
            ViewData["UserID"] = userID;
            ViewData["IsAdmin"] = isAdmin == 1;

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        

        [HttpPost]
        public IActionResult addClaim(int hoursWorked, int hourlyRate, IFormFile SupportingDocuments)
        {
            int? userID = HttpContext.Session.GetInt32("userID");
            if (userID == null)
            {
                // Handle the case where the user is not logged in
                return RedirectToAction("Login", "Home");
            }

            // Save the uploaded file and get the file URL
            string fileURL = SaveFile(SupportingDocuments);

            if (string.IsNullOrEmpty(fileURL))
            {
                // Handle the case where the file URL is not set
                ModelState.AddModelError("", "File upload failed. Please try again.");
                return View("Privacy");
            }

            // Create a new claim
            ClaimTBL claim = new ClaimTBL
            {
                userID = userID.Value,
                fileURL = fileURL,
                hoursWorked = hoursWorked,
                hourlyrate = hourlyRate,
                amountDue = hoursWorked * hourlyRate,
                claimStatus = "pending"
            };

            // Insert the claim into the database
            ClaimTBL claimTbl = new ClaimTBL();
            int rowsAffected = claimTbl.insert_Claim(claim);

            if (rowsAffected == 0)
            {
                // Handle the case where the claim was not inserted
                ModelState.AddModelError("", "Failed to insert claim. Please try again.");
                return View("Privacy");
            }

            return RedirectToAction("Index", "Home");
        }

        //github copilot assisted me with the logic for this method
        private string SaveFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return null;
            }

            string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
            Directory.CreateDirectory(uploadsFolder);
            string uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(fileStream);
            }

            return "/uploads/" + uniqueFileName;
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