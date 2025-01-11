using Microsoft.AspNetCore.Mvc;
using Assignment3_ShongChan.Data;
using Assignment3_ShongChan.Models.DomainModels;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace Assignment3_ShongChan.Controllers
{
    public class ImageController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ImageController(AppDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            var images = _context.Images.ToList();
            return View(images);
        }

        [HttpGet]
        public IActionResult Display(int id)
        {
            var image = _context.Images.FirstOrDefault(i => i.Id == id);
            if (image != null && !string.IsNullOrEmpty(image.ImagePath))
            {
                var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, image.ImagePath.TrimStart('/'));
                if (System.IO.File.Exists(imagePath))
                {
                    var imageStream = System.IO.File.OpenRead(imagePath);
                    return File(imageStream, image.ContentType);
                }
            }
            return NotFound("Image not found.");
        }

        [HttpGet]
        public IActionResult UploadImage()
        {
            return View();
        }

        [HttpPost]
        public IActionResult UploadImage(IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
            {
                TempData["Error"] = "No file selected. Please choose an image.";
                return RedirectToAction("UploadImage");
            }

            try
            {
                // Generate a unique file name and save the image
                string uploadFolder = Path.Combine(_webHostEnvironment.WebRootPath, "Images");
                Directory.CreateDirectory(uploadFolder);

                string uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(imageFile.FileName)}";
                string filePath = Path.Combine(uploadFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    imageFile.CopyTo(fileStream);
                }

                // Update user profile picture in the database
                var username = HttpContext.Session.GetString("Username");
                var user = _context.Users.FirstOrDefault(u => u.Username == username);

                if (user != null)
                {
                    user.ProfilePic = $"/Images/{uniqueFileName}";
                    _context.Users.Update(user);
                    _context.SaveChanges();

                    TempData["Message"] = "Profile picture uploaded successfully.";
                    return RedirectToAction("Index", "Profile"); 
                }
                else
                {
                    TempData["Error"] = "User not found.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"An error occurred: {ex.Message}";
            }

            return RedirectToAction("UploadImage");
        }
    }
}
