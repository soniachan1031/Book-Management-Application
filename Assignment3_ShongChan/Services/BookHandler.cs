using Microsoft.EntityFrameworkCore;
using Assignment3_ShongChan.Data;
using Assignment3_ShongChan.Models.DomainModels;

namespace Assignment3_ShongChan.Services
{
    public class BookHandler
    {
        private readonly AppDbContext _context;

        public BookHandler(AppDbContext context)
        {
            _context = context;
        }

        public List<Book> GetAllBooks()
        {
            return _context.Books.Include(b => b.Genre).ToList();
        }

        // Get a book by id
        public List<Book> SearchBooks(string searchTerm)
        {
            // Start with the base query
            var query = _context.Books
                .Include(b => b.Genre) // Ensure Genre is included for navigation property
                .AsQueryable(); // Convert to IQueryable to allow for dynamic filtering

            // Filter by search term
            if (!string.IsNullOrEmpty(searchTerm))
            {
                // Filter by Title, Author, and Genre Name
                query = query.Where(b =>
                    (b.Title != null && b.Title.ToLower().Contains(searchTerm.ToLower())) ||
                    (b.Author != null && b.Author.ToLower().Contains(searchTerm.ToLower())) ||
                    (b.Genre != null && b.Genre.Name != null && b.Genre.Name.ToLower().Contains(searchTerm.ToLower())));
            }

            return query.ToList();
        }

        // Filter books by genre
        public List<Book> FilterByGenre(List<Book> books, string genre)
        {
            if (string.IsNullOrEmpty(genre))
                return books;
            return books.Where(b => b.Genre != null && b.Genre.Name != null && b.Genre.Name.Equals(genre, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        // Filter books by availability
        public List<Book> FilterByAvailability(List<Book> books, bool? available)
        {
            if (available == null) return books;
            return available == true ? books.Where(b => b.IsAvailable).ToList() : books.Where(b => !b.IsAvailable).ToList();
        }

        // Sort books by a given property
        public List<Book> SortBooks(List<Book> books, string sortBy, string sortOrder)
        {
            return sortBy switch
            {
                "title" => sortOrder == "asc" ? books.OrderBy(b => b.Title).ToList() : books.OrderByDescending(b => b.Title).ToList(),
                "author" => sortOrder == "asc" ? books.OrderBy(b => b.Author).ToList() : books.OrderByDescending(b => b.Author).ToList(),
                "genre" => sortOrder == "asc" ? books.OrderBy(b => b.Genre != null ? b.Genre.Name : string.Empty).ToList() : books.OrderByDescending(b => b.Genre != null ? b.Genre.Name : string.Empty).ToList(),
                "year" => sortOrder == "asc" ? books.OrderBy(b => b.Year).ToList() : books.OrderByDescending(b => b.Year).ToList(),
                "quantity" => sortOrder == "asc" ? books.OrderBy(b => b.Quantity).ToList() : books.OrderByDescending(b => b.Quantity).ToList(),
                "availableQuantity" => sortOrder == "asc" ? books.OrderBy(b => b.AvailableQuantity).ToList() : books.OrderByDescending(b => b.AvailableQuantity).ToList(),
                "isAvailable" => sortOrder == "asc" ? books.OrderBy(b => b.IsAvailable).ToList() : books.OrderByDescending(b => b.IsAvailable).ToList(),
                _ => books
            };
        }

        // Get all genres
        public List<string> GetGenres()
        {
            return _context.Genres.Select(g => g.Name).ToList();
        }

        public List<Book> GetAvailableBooks()
        {
            return _context.Books
                .Include(b => b.Genre)
                .Where(b => b.AvailableQuantity > 0 && b.IsAvailable)
                .ToList();
        }
    }
}
