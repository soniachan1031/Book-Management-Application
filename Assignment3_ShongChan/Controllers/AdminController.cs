using Assignment3_ShongChan.Models.DomainModels;
using Assignment3_ShongChan.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Assignment3_ShongChan.Data;
using Assignment3_ShongChan.Services;
using Microsoft.EntityFrameworkCore;
using System.Linq;


namespace Assignment3_ShongChan.Controllers
{
    [Route("admin")]
    public class AdminController : Controller
    {
        private readonly AuthHandler _auth;
        private readonly BookHandler _bookHandler;
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AdminController(AuthHandler direct, BookHandler bookHandler, AppDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _auth = direct;
            _bookHandler = bookHandler;
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // Admin Dashboard
        [HttpGet("")]
        public IActionResult Index()
        {
            var user = _auth.GetUser();
            if (user == null || !_auth.IsAdmin(user))
            {
                TempData["ErrorMessage"] = "Access denied. Admins only.";
                return RedirectToAction("Login", "Home");
            }

            ViewBag.ProfilePic = string.IsNullOrEmpty(user.ProfilePic) ? "/Images/default-profile.png" : user.ProfilePic;
            ViewBag.Username = user.Username;
            return View(user);
        }

        // Add Member
        [HttpGet("AddMember")]
        public IActionResult AddMember()
        {
            return View(new RegisterViewModel());
        }

        [HttpPost("AddMember")]
        public IActionResult AddMember(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var member = new User
                {
                    Username = model.Username,
                    Password = model.Password,
                    ConfirmPassword = model.ConfirmPassword,
                    Name = model.Name,
                    DateOfBirth = model.DateOfBirth ?? DateTime.Now,
                    Age = model.Age ?? string.Empty, // Handle possible null reference
                    Role = "Member",
                    ProfilePic = model.ProfileImage != null ? UploadProfileImage(model.ProfileImage) : "/Images/default-profile.png"
                };

                _context.Users.Add(member);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Member added successfully!";
                return RedirectToAction("MemberList");
            }
            return View("AddMember", model);
        }


        // Upload Profile Image
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


        // Edit Member
        [HttpGet("EditMember/{id}")]
        public async Task<IActionResult> EditMember(int id)
        {
            var member = await _context.Users.FindAsync(id); 
            if (member == null)
            {
                return NotFound();
            }

            var model = new RegisterViewModel
            {
                Id = member.Id,
                Username = member.Username,
                Name = member.Name,
                DateOfBirth = member.DateOfBirth,
                Age = member.Age ?? string.Empty,
                ProfilePic = member.ProfilePic 
            };

            return View("AddMember", model);
        }


        [HttpPost("EditMember/{id}")]
        public async Task<IActionResult> EditMember(int id, RegisterViewModel model, IFormFile? profileImage)
        {
            ModelState.Remove("Password");
            ModelState.Remove("ConfirmPassword");

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                foreach (var error in errors)
                {
                    ModelState.AddModelError(string.Empty, error);
                }
            }

            if (ModelState.IsValid)
            {
                var member = await _context.Users.FindAsync(id);
                if (member == null)
                {
                    return NotFound();
                }

                // Update member details
                member.Username = model.Username;
                member.Name = model.Name;
                member.DateOfBirth = model.DateOfBirth ?? DateTime.Now;
                member.Age = model.Age ?? string.Empty;

                if (profileImage != null && profileImage.Length > 0)
                {
                    string uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(profileImage.FileName)}";
                    string uploadFolder = Path.Combine(_webHostEnvironment.WebRootPath, "Images");
                    string filePath = Path.Combine(uploadFolder, uniqueFileName);

                    if (!Directory.Exists(uploadFolder))
                    {
                        Directory.CreateDirectory(uploadFolder);
                    }

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await profileImage.CopyToAsync(stream);
                    }

                    member.ProfilePic = $"/Images/{uniqueFileName}";
                }
                else
                {
                    member.ProfilePic = member.ProfilePic ?? "/Images/default-profile.png";
                }

                try
                {
                    _context.Users.Update(member);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Member updated successfully!";
                    return RedirectToAction("MemberList");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, "An error occurred while saving the member: " + ex.Message);
                }
            }

            return View("AddMember", model);
        }


        // Delete Member
        [HttpPost("DeleteMember/{id}")]
        public IActionResult DeleteMember(int id)
        {
            var member = _context.Users.Include(u => u.Book).FirstOrDefault(u => u.Id == id);
            if (member == null)
            {
                TempData["ErrorMessage"] = "Member not found.";
                return RedirectToAction("MemberList");
            }

            if (member.BookId.HasValue)
            {
                TempData["ErrorMessage"] = "Cannot delete member with an assigned book.";
                return RedirectToAction("MemberList");
            }

            _context.Users.Remove(member);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Member deleted successfully.";
            return RedirectToAction("MemberList");
        }


        // Add Book
        [HttpGet("add")]
        public IActionResult AddBook()
        {
            ViewBag.Genres = _context.Genres.ToList();
            ViewBag.Action = "AddBook";
            return View("AddBook", new Book());
        }


        [HttpPost("add")]
        public IActionResult AddBook(Book book)
        {
            if (ModelState.IsValid)
            {
                book.AvailableQuantity = book.Quantity;
                book.IsAvailable = book.Quantity > 0;

                _context.Books.Add(book);
                _context.SaveChanges();
                return RedirectToAction("Books");
            }

            ViewBag.Genres = _context.Genres.ToList();
            ViewBag.Action = "AddBook";
            return View("AddBook", book);
        }


        // Edit Book
        [HttpGet("edit/{id}")]
        public IActionResult EditBook(int id)
        {
            var book = _context.Books.Include(b => b.Genre).FirstOrDefault(b => b.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            ViewBag.Genres = _context.Genres.ToList();
            ViewBag.Action = "EditBook";
            return View("AddBook", book);
        }


        [HttpPost("edit/{id}")]
        public IActionResult EditBook(Book book)
        {
            if (ModelState.IsValid)
            {
                var existingBook = _context.Books.FirstOrDefault(b => b.Id == book.Id);
                if (existingBook != null)
                {
                    if (book.Quantity < existingBook.AvailableQuantity)
                    {
                        ModelState.AddModelError(nameof(book.Quantity), "Quantity cannot be less than available quantity.");
                        ViewBag.Genres = _context.Genres.ToList();
                        ViewBag.Action = "EditBook";
                        return View("AddBook", book);
                    }

                    existingBook.Title = book.Title;
                    existingBook.Author = book.Author;
                    existingBook.GenreId = book.GenreId;
                    existingBook.Year = book.Year;
                    existingBook.Quantity = book.Quantity;

                    if (book.Quantity > existingBook.Quantity)
                    {
                        existingBook.AvailableQuantity += (book.Quantity - existingBook.Quantity);
                    }

                    existingBook.IsAvailable = existingBook.AvailableQuantity > 0;

                    _context.SaveChanges();
                    return RedirectToAction("Books");
                }
            }

            ViewBag.Genres = _context.Genres.ToList();
            ViewBag.Action = "EditBook";
            return View("AddBook", book);
        }


        // Delete Book
        [HttpGet("DeleteBook/{id}")]
        public IActionResult DeleteBook(int id)
        {
            var book = _context.Books.Include(b => b.Genre).FirstOrDefault(b => b.Id == id);
            if (book == null)
            {
                return    NotFound();
            }

            if (book.AvailableQuantity != book.Quantity)
            {
                ViewBag.Warning = $"This book '{book.Title}' has been assigned or issued. Please unassign or return it before deletion.";
                ViewBag.UnassignBook = Url.Action("ReturnBook", "Admin");
                return View(book);
            }

            return View(book);
        }


        [HttpPost("DeleteBook/{id}")]
        public IActionResult DeleteBookConfirmed(int id)
        {
            var book = _context.Books.Find(id);
            if (book == null)
            {
                return NotFound();
            }

            _context.Books.Remove(book);
            _context.SaveChanges();
            TempData["SuccessMessage"] = "Book deleted successfully.";
            return RedirectToAction("Index");
        }


        // Issue Book
        [HttpGet("IssueBook")]
        public IActionResult IssueBook()
        {
            var model = new IssuedBookViewModel
            {
                Members = _context.Users
                          .Where(m => m.BookId == null && m.Username != "admin") 
                          .ToList(),
                Books = _context.Books.Where(b => b.AvailableQuantity > 0).ToList()
            };

            return View(model);
        }



        [HttpPost("IssueBook")]
        public IActionResult IssueBook(int memberId, int bookId)
        {
            var member = _context.Users.Include(m => m.Book).FirstOrDefault(m => m.Id == memberId);
            var book = _context.Books.FirstOrDefault(b => b.Id == bookId);

            // Validate input
            if (member == null || book == null)
            {
                TempData["ErrorMessage"] = "Invalid request. Either the member or the book is not found.";
                return RedirectToAction("MemberList");
            }

            if (book.BorrowedById != null)
            {
                TempData["ErrorMessage"] = $"Book '{book.Title}' is already assigned to another member.";
                return RedirectToAction("MemberList");
            }

            // Assign the book to the member
            book.BorrowedById = memberId;
            book.AvailableQuantity--;
            book.IsAvailable = book.AvailableQuantity > 0;

            member.BookId = bookId;

            // Update database
            _context.Users.Update(member);
            _context.Books.Update(book);
            _context.SaveChanges();

            // Feedback to the admin
            TempData["SuccessMessage"] = $"Book '{book.Title}' assigned to '{member.Name}' successfully.";
            return RedirectToAction("MemberList");
        }


        [HttpGet("ReturnBook")]
        public IActionResult ReturnBook()
        {
            var issuedBooks = _context.Users
                .Include(u => u.Book)
                .Where(u => u.BookId.HasValue)
                .Select(u => new IssuedBookViewModel
                {
                    MemberName = u.Name,
                    BookTitle = u.Book != null ? u.Book.Title : "Unknown",
                    MemberId = u.Id,
                    BookId = u.BookId.Value
                }).ToList();

            return View(issuedBooks);
        }


        [HttpPost("ReturnBook")]
        public IActionResult ReturnBookConfirmed(int memberId, int bookId)
        {
            var member = _context.Users.Include(m => m.Book).FirstOrDefault(m => m.Id == memberId);
            var book = _context.Books.FirstOrDefault(b => b.Id == bookId);

            if (member == null || book == null || member.BookId != bookId)
            {
                TempData["ErrorMessage"] = "Invalid return request. Either member or book is not found, or the book is not assigned to this member.";
                return RedirectToAction("ReturnBook");
            }

            member.BookId = null; // Clear the member's assigned book
            book.BorrowedById = null; // Clear the book's borrower
            book.AvailableQuantity++;
            book.IsAvailable = true;

            _context.Users.Update(member);
            _context.Books.Update(book);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Book return confirmed successfully!";
            return RedirectToAction("ReturnBook");
        }


        // Books List
        [HttpGet("books")]
        public IActionResult Books()
        {
            var books = _context.Books.Include(b => b.Genre).ToList();
            var model = new SearchBookViewModel
            {
                Books = books,
                Genres = _context.Genres.ToList(),
                SearchTerm = string.Empty,
                GenreFilter = string.Empty,
                AvailabilityFilter = string.Empty,
                SortBy = string.Empty,
                SortOrder = string.Empty
            };

            return View(model);
        }


        [HttpGet("MemberList")]
        public IActionResult MemberList()
        {
            var members = _context.Users
                          .Include(u => u.Book)
                          .Where(u => u.Role != "Admin")
                          .ToList();
            return View(members);
        }
    }
}

