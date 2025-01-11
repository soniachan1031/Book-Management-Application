using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Assignment3_ShongChan.Data;
using Assignment3_ShongChan.Models;
using Assignment3_ShongChan.Services;
using Assignment3_ShongChan.Models.DomainModels;
using Assignment3_ShongChan.Models.ViewModels;

namespace Assignment3_ShongChan.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;
        private readonly AuthHandler _auth;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public HomeController(AppDbContext context, AuthHandler auth, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _auth = auth;
            _webHostEnvironment = webHostEnvironment;
        }

        public bool IsSessionActive()
        {
            var sessionStart = HttpContext.Session.GetString("SessionStart");

            if (string.IsNullOrEmpty(sessionStart) || !DateTime.TryParse(sessionStart, out var startTime))
            {
                TempData["SessionExpired"] = "Your session has expired. Please log in.";
                return false;
            }

            if ((DateTime.Now - startTime).TotalSeconds > 30)
            {
                HttpContext.Session.Clear();
                TempData["SessionExpired"] = "Your session has expired. Please log in.";
                return false;
            }

            HttpContext.Session.SetString("SessionStart", DateTime.Now.ToString());
            return true;
        }

        public IActionResult Index()
        {
            if (!IsSessionActive())
            {
                TempData["SessionExpired"] = "Your session has expired. Please log in.";
                return RedirectToAction("Login", "Home");
            }

            if (!_auth.IsLoggedIn())
            {
                return RedirectToAction("Login");
            }

            ViewBag.Username = HttpContext.Session.GetString("Username");
            string controller = _auth.IsAdmin() ? "Admin" : "Member";
            return RedirectToAction("Index", controller);
        }


        [HttpGet("register")]
        public IActionResult Register()
        {
            if (_auth.IsLoggedIn())
            {
                string controller = _auth.IsAdmin() ? "Admin" : "Member";
                return RedirectToAction("Index", controller);
            }
            return View(new RegisterViewModel());
        }


        [HttpPost("register")]
        public IActionResult Register(RegisterViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                // Populate User object from ViewModel
                var user = new User
                {
                    Username = viewModel.Username,
                    Password = viewModel.Password,
                    Name = viewModel.Name,
                    Age = viewModel.Age,
                    DateOfBirth = viewModel.DateOfBirth ?? DateTime.Now, // Use a default value
                    Role = "Member", // Default to Member role
                    ProfilePic = viewModel.ProfileImage != null ? UploadProfileImage(viewModel.ProfileImage) : "/Images/default-profile.png"
                };

                _context.Users.Add(user);
                _context.SaveChanges();

                TempData["SuccessMessage"] = "Registration successful! Please login.";
                return RedirectToAction("Login");
            }
            return View(viewModel);
        }



        [HttpGet("login")]
        public IActionResult Login()
        {
            if (_auth.IsLoggedIn())
            {
                string controller = _auth.IsAdmin() ? "Admin" : "Member";
                return RedirectToAction("Index", controller);
            }
            return View();
        }


        [HttpPost("login")]
        public IActionResult Login(string username, string password)
        {
            var user = _auth.GetUserByCredentials(username, password);
            if (user == null)
            {
                ViewBag.ErrorMessage = "Invalid username or password.";
                return View();
            }

            // Set session variables
            HttpContext.Session.SetString("Username", user.Username);
            HttpContext.Session.SetString("UserId", user.Id.ToString());
            HttpContext.Session.SetString("Role", user.Role);

            // Redirect based on role
            if (user.Role == "Admin")
            {
                return RedirectToAction("Index", "Admin");
            }
            else if (user.Role == "Member")
            {
                return RedirectToAction("Index", "Member");
            }

            return View(); 
        }



        [HttpGet("logout")]
        public IActionResult Logout()
        {
            _auth.Logout();
            return RedirectToAction("Login");
        }


        public IActionResult Handle404()
        {
            return View("NotFound");
        }


        // Action to serve the image
        [HttpGet("images/{fileName}")]
        public IActionResult GetImage(string fileName)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }
            var image = System.IO.File.OpenRead(filePath);
            return File(image, "image/jpeg");
        }

        private string UploadProfileImage(IFormFile profileImage)
        {
            if (profileImage != null && profileImage.Length > 0)
            {
                string uploadFolder = Path.Combine(_webHostEnvironment.WebRootPath, "Images");
                Directory.CreateDirectory(uploadFolder);

                string uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(profileImage.FileName)}";
                string filePath = Path.Combine(uploadFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    profileImage.CopyTo(stream);
                }

                return "/Images/" + uniqueFileName;
            }

            return "/Images/default-profile.png";
        }
    }
}
