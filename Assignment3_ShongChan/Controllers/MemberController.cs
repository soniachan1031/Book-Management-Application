using Assignment3_ShongChan.Models.ViewModels;
using Assignment3_ShongChan.Services;
using Assignment3_ShongChan.Data;
using Assignment3_ShongChan.Models.DomainModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace Assignment3_ShongChan.Controllers
{
    [Route("member")]
    public class MemberController : Controller
    {
        private readonly AuthHandler _auth;
        private readonly BookHandler _bookHandler;
        private readonly AppDbContext _context;

        public MemberController(AuthHandler auth, AppDbContext context)
        {
            _auth = auth;
            _bookHandler = new BookHandler(context);
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var user = _auth.GetUser();
            if (user == null)
                return RedirectToAction("Login", "Home");
            if (_auth.IsAdmin(user))
                return RedirectToAction("Index", "Admin");
            return View(user);
        }

        // Search Books
        [HttpGet("books")]
        public IActionResult Books(string searchTerm, string genre, string available, string sortBy, string sortOrder = "asc")
        {
            ViewBag.Action = "SearchBooks";

            var books = _bookHandler.SearchBooks(searchTerm);
            books = _bookHandler.FilterByGenre(books, genre);

            bool? isAvailable = null;
            if (available == "true")
            {
                isAvailable = true;
                books = _bookHandler.FilterByAvailability(books, isAvailable);
            }
            else if (available == "false")
            {
                isAvailable = false;
                books = _bookHandler.FilterByAvailability(books, isAvailable);
            }

            books = _bookHandler.SortBooks(books, sortBy, sortOrder);
            var genres = _context.Genres.Where(g => !string.IsNullOrEmpty(g.Name)).Distinct().ToList();

            var viewModel = new SearchBookViewModel
            {
                Books = books,
                Genres = genres,
                SearchTerm = searchTerm,
                SelectedGenre = genre,
                SortBy = sortBy,
                SortOrder = sortOrder,
                Available = isAvailable
            };
            return View(viewModel);
        }

        [HttpGet("clearsearch")]
        public IActionResult ClearSearch()
        {
            return RedirectToAction("Books");
        }


        // Issue Book
        [HttpGet("issuebook")]
        public IActionResult IssueBook()
        {
            var user = _auth.GetUser();
            if (user == null)
                return RedirectToAction("Login", "Home");

            // Fetch available books (those not currently borrowed)
            var availableBooks = _context.Books
                .Where(b => b.BorrowedBy == null && b.AvailableQuantity > 0)
                .Include(b => b.Genre)
                .ToList();

            return View(availableBooks); 
        }


        [HttpPost("issuebook/{id}")]
        public IActionResult IssueBook(int id)
        {
            var user = _auth.GetUser();
            if (user == null)
                return RedirectToAction("Login", "Home");

            // Check if the user already has an issued book
            if (_context.Books.Any(b => b.BorrowedById == user.Id)) // Use the foreign key
            {
                ViewBag.ErrorMessage = "You have already issued a book. Please return the book before issuing another one.";
                var availableBooks = _context.Books
                    .Where(b => b.BorrowedBy == null && b.AvailableQuantity > 0)
                    .Include(b => b.Genre)
                    .ToList();
                return View("IssueBook", availableBooks);
            }

            var book = _context.Books.FirstOrDefault(b => b.Id == id && b.AvailableQuantity > 0);
            if (book == null)
            {
                ViewBag.ErrorMessage = "The selected book is not available.";
                var availableBooks = _context.Books
                    .Where(b => b.BorrowedBy == null && b.AvailableQuantity > 0)
                    .Include(b => b.Genre)
                    .ToList();
                return View("IssueBook", availableBooks);
            }

            // Assign the book to the user
            book.BorrowedBy = user;
            book.AvailableQuantity--;
            book.IsAvailable = book.AvailableQuantity > 0;

            user.BookId = book.Id;

            _context.Users.Update(user);
            _context.Books.Update(book);
            _context.SaveChanges();

            ViewBag.SuccessMessage = $"You have successfully issued the book '{book.Title}'.";
            var updatedBooks = _context.Books
                .Where(b => b.BorrowedBy == null && b.AvailableQuantity > 0)
                .Include(b => b.Genre)
                .ToList();
            return View("IssueBook", updatedBooks);
        }



        // Return Book
        [HttpGet("returnbook")]
        public IActionResult ReturnBook()
        {
            var user = _auth.GetUser();
            if (user == null)
                return RedirectToAction("Login", "Home");

            var issuedBooks = _context.Books
                .Where(b => b.BorrowedById == user.Id)
                .Include(b => b.Genre)
                .ToList();

            return View("ReturnBook", issuedBooks); 
        }


        [HttpPost("returnbook/{id}")]
        public IActionResult ReturnBook(int id)
        {
            var user = _auth.GetUser();
            if (user == null)
                return RedirectToAction("Login", "Home");

            var book = _context.Books.FirstOrDefault(b => b.Id == id && b.BorrowedById == user.Id);
            if (book == null)
            {
                TempData["ErrorMessage"] = "You have not issued this book or it does not exist.";
                return RedirectToAction("ReturnBook");
            }

            // Update the book properties
            book.BorrowedBy = null;
            book.AvailableQuantity++;
            book.IsAvailable = true;

            // Update the user record (clear the assigned BookId)
            var member = _context.Users.FirstOrDefault(u => u.Id == user.Id);
            if (member != null)
            {
                member.BookId = null; 
                _context.Users.Update(member);
            }

            _context.Books.Update(book);
            _context.SaveChanges();

            TempData["SuccessMessage"] = $"You have successfully returned the book '{book.Title}'.";
            return RedirectToAction("MyBooks");
        }



        // List Issued Book
        [HttpGet("mybooklist")]
        public IActionResult MyBooks()
        {
            var user = _auth.GetUser();
            if (user == null) return RedirectToAction("Login", "Home");

            var issuedBooks = _context.Books
                .Include(b => b.Genre)
                .Where(b => b.BorrowedById == user.Id)
                .ToList();

            return View("MyBookList", issuedBooks);
        }
    }
}
