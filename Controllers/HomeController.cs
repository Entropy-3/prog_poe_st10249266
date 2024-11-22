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

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\\
        //method that autopopulates the home page with the users claims
        public IActionResult Index()
        {
            int? userID = HttpContext.Session.GetInt32("userID");
            int? isAdmin = HttpContext.Session.GetInt32("isAdmin");
            ViewData["UserID"] = userID;
            ViewData["IsAdmin"] = isAdmin == 1;

            if (userID == null)
            {
                // Handle the case where the user is not logged in
                return RedirectToAction("Login", "Home");
            }

            List<ClaimTBL> claims = ClaimTBL.getClaimsByUserId(userID.Value);
            return View(claims);
        }

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\\
        public IActionResult Privacy()
        {
            int? userID = HttpContext.Session.GetInt32("userID");
            ViewData["UserID"] = userID;
            return View();
        }


        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\\
        //method that allows the user to add a claim
        [HttpPost]
        public IActionResult addClaim(int hoursWorked, int hourlyRate, IFormFile SupportingDocuments)
        {
            int? userID = HttpContext.Session.GetInt32("userID");
            if (userID == null)
            {
                return RedirectToAction("Login", "Home");
            }

            // Validate that hoursWorked and hourlyRate are not negative
            if (hoursWorked < 0 || hourlyRate < 0)
            {
                ModelState.AddModelError("", "Hours worked and hourly rate must be non-negative values.");
                return View("Privacy");
            }

            // Save the uploaded file and get the file URL
            string fileURL = SaveFile(SupportingDocuments);

            if (string.IsNullOrEmpty(fileURL))
            {
                ModelState.AddModelError("", "File upload failed. Please try again.");
                return View("Privacy");
            }

            ClaimTBL claim = new ClaimTBL
            {
                userID = userID.Value,
                fileURL = fileURL,
                hoursWorked = hoursWorked,
                hourlyrate = hourlyRate,
                amountDue = hoursWorked * hourlyRate,
                claimStatus = "pending"
            };

            ClaimTBL claimTbl = new ClaimTBL();
            int rowsAffected = claimTbl.insertClaim(claim);

            if (rowsAffected == 0)
            {
                ModelState.AddModelError("", "Failed to insert claim. Please try again.");
                return View("Privacy");
            }

            return RedirectToAction("Index", "Home");
        }

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\\
        //github copilot assisted me with the logic for this method
        //method that saves the file to root and returns the file path
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

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\\
        //method that allows the user to view claims
        public IActionResult ViewClaims()
        {
            int? userID = HttpContext.Session.GetInt32("userID");
            ViewData["UserID"] = userID;
            if (userID == null)
            {
                // Handle the case where the user is not logged in
                return RedirectToAction("Login", "Home");
            }

            List<ClaimTBL> claims = ClaimTBL.getPendingClaims();
            return View(claims);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public UserTBL usrtbl = new UserTBL();

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\\
        [HttpPost]
        public ActionResult SignUp(UserTBL Users)
        {
            var result = usrtbl.insert_User(Users);
            return RedirectToAction("Index", "Home");
        }

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\\
        //allows user to logout
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("userID");
            return RedirectToAction("Index", "Home");
        }


        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\\
        //allows the user to sign up
        [HttpGet]
        public ActionResult SignUp()
        {
            return View(usrtbl);
        }

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\\
        //allows the user to login
        public ActionResult Login()
        {
            return View(usrtbl);
        }

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\\
        //method that allows the admin to approve a claim
        [HttpPost]
        public IActionResult ApproveClaim(int claimID)
        {
            ClaimTBL claim = new ClaimTBL();
            claim.UpdateClaimStatus(claimID, "Approved");
            return RedirectToAction("ViewClaims");
        }

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\\
        //allows the admin to reject a claim
        [HttpPost]
        public IActionResult RejectClaim(int claimID)
        {
            ClaimTBL claim = new ClaimTBL();
            claim.UpdateClaimStatus(claimID, "Rejected");
            return RedirectToAction("ViewClaims");
        }







        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\\
        //method that automatically processes claims based on user input
        [HttpPost]
        public IActionResult AutoProcessClaims(int minHoursWorked, int maxHourlyRate)
        {
            List<ClaimTBL> claims = ClaimTBL.getPendingClaims();

            foreach (var claim in claims)
            {
                if (claim.hoursWorked >= minHoursWorked && claim.hourlyrate <= maxHourlyRate)
                {
                    claim.UpdateClaimStatus(claim.claimID, "Approved");
                }
                else
                {
                    claim.UpdateClaimStatus(claim.claimID, "Rejected");
                }
            }

            return RedirectToAction("ViewClaims");
        }

    }
}
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~EOF~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\\