using Microsoft.AspNetCore.Mvc;
using Assignment3_ShongChan.Data;
using Assignment3_ShongChan.Models.DomainModels;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Assignment3_ShongChan.Models.ViewModels;
using Assignment3_ShongChan.Services;

namespace Assignment3_ShongChan.Controllers
{
    public class ProfileController : Controller
    {
        private readonly AuthHandler _auth;
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProfileController(AuthHandler auth, AppDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _auth = auth;
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // Upload profile image
        private string UploadProfileImage(IFormFile? profileImage)
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

        // Profile
        [HttpGet]
        public IActionResult Index()
        {
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                TempData["Error"] = "Your session has expired. Please log in.";
                return RedirectToAction("Login", "Home");
            }

            var user = _context.Users.FirstOrDefault(u => u.Username == username);
            if (user == null)
            {
                TempData["Error"] = "User not found.";
                return RedirectToAction("Login", "Home");
            }

            return View(user);
        }



        [HttpPost]
        public IActionResult Index(User user, IFormFile profileImage)
        {
            try
            {
                var existingUser = _context.Users.FirstOrDefault(u => u.Id == user.Id);
                if (existingUser == null)
                {
                    ViewBag.Error = "User not found.";
                    return View(user);
                }

                existingUser.Name = user.Name;
                existingUser.Age = user.Age;
                existingUser.DateOfBirth = user.DateOfBirth;

                if (profileImage != null)
                {
                    existingUser.ProfilePic = UploadProfileImage(profileImage);
                }

                _context.Users.Update(existingUser);
                _context.SaveChanges();

                ViewData["Message"] = "Profile updated successfully.";
                return View(existingUser);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "An error occurred while saving the profile: " + ex.Message;
                return View(user);
            }
        }

        // Image Upload
        [HttpGet]
        public IActionResult UploadImage()
        {
            return View();
        }


        [HttpPost]
        public IActionResult UploadImage(IFormFile profileImage)
        {
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                TempData["Error"] = "Your session has expired. Please log in again.";
                return RedirectToAction("Login", "Home");
            }

            var user = _context.Users.FirstOrDefault(u => u.Username == username);
            if (user == null)
            {
                TempData["Error"] = "User not found.";
                return RedirectToAction("Index");
            }

            if (profileImage != null && profileImage.Length > 0)
            {
                try
                {
                    string uploadFolder = Path.Combine(_webHostEnvironment.WebRootPath, "Images");
                    Directory.CreateDirectory(uploadFolder);

                    string uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(profileImage.FileName)}";
                    string filePath = Path.Combine(uploadFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        profileImage.CopyTo(stream);
                    }

                    user.ProfilePic = $"/Images/{uniqueFileName}";
                    _context.Users.Update(user);
                    _context.SaveChanges();

                    TempData["Message"] = "Profile picture uploaded successfully.";
                }
                catch (Exception ex)
                {
                    TempData["Error"] = $"An error occurred: {ex.Message}";
                }
            }
            else
            {
                TempData["Error"] = "No file selected. Please choose an image.";
            }

            return RedirectToAction("Index");
        }




        // Change Password
        [HttpGet]
        public IActionResult ChangePassword()
        {
            var username = HttpContext.Session.GetString("Username");

            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Home");
            }

            var model = new ChangePasswordViewModel
            {
                Username = username
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                TempData["Error"] = "Your session has expired. Please log in again.";
                return RedirectToAction("Login", "Home");
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null)
            {
                ModelState.AddModelError("", "User not found.");
                return View(model);
            }

            if (user.Password != model.OldPassword)
            {
                ModelState.AddModelError("OldPassword", "Old password is incorrect.");
                return View(model);
            }

            if (model.NewPassword != model.ConfirmPassword)
            {
                ModelState.AddModelError("ConfirmPassword", "New password and confirm password do not match.");
                return View(model);
            }

            user.Password = model.NewPassword;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            TempData["Message"] = "Password changed successfully.";
            return RedirectToAction("Index","Profile");
        }
    }
}