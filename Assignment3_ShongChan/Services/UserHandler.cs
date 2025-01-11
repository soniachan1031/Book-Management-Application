using Assignment3_ShongChan.Models.DomainModels;
using Microsoft.EntityFrameworkCore;
using Assignment3_ShongChan.Data;

namespace Assignment3_ShongChan.Services
{
    public class UserHandler
    {
        private readonly AppDbContext _context;

        public UserHandler(AppDbContext context)
        {
            _context = context;
        }

        public List<User> GetAllUsers()
        {
            return _context.Users.Include(u => u.Book).ToList();
        }

        public List<User> SearchUsers(string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
                return _context.Users.Include(u => u.Book).ToList();

            searchTerm = searchTerm.ToLower();
            return _context.Users.Include(u => u.Book)
                .Where(u => u.Username != null && u.Username.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        // Filter users by book assignment
        public List<User> FilterByBookAssignment(List<User> users, bool? hasBookAssigned)
        {
            if (hasBookAssigned == null)
                return users;
            return hasBookAssigned == true ? users.Where(u => u.BookId != null).ToList() : users.Where(u => u.BookId == null).ToList();
        }

        // Sort users by a given property
        public List<User> SortMembers(List<User> users, string sortOrder)
        {
            return sortOrder == "desc" ? users.OrderByDescending(u => u.Username).ToList() : users.OrderBy(u => u.Username).ToList();
        }
    }
}

