using Microsoft.AspNetCore.Http;
using Assignment3_ShongChan.Data;
using Assignment3_ShongChan.Models.DomainModels;

namespace Assignment3_ShongChan.Services
{
    // This class is used to handle authentication and authorization
    public class AuthHandler
    {
        private readonly HttpContext? _context;
        private readonly AppDbContext _db;

        public AuthHandler(IHttpContextAccessor accessor, AppDbContext context)
        {
            _context = accessor.HttpContext; 
            _db = context; 
        }

        public bool IsLoggedIn()
        {
            var userId = _context?.Session.GetString("UserId");
            return !string.IsNullOrEmpty(userId) && _db.Users.Any(u => u.Id.ToString() == userId);
        }

        public bool IsAdmin()
        {
            var userId = _context?.Session.GetString("UserId"); 
            return userId != null && _db.Users.Any(u => u.Id.ToString() == userId && u.Role == "Admin");
        }

        public bool IsAdmin(User user) => user.Role.Equals("Admin", StringComparison.OrdinalIgnoreCase);

        public bool IsMember()
        {
            var userId = _context?.Session.GetString("UserId");
            return userId != null && _db.Users.Any(u => u.Id.ToString() == userId && u.Role == "Member");
        }

        public bool IsMember(User user) => user.Role == "Member";

        // Get the current user
        public User? GetUser()
        {
            var userId = _context?.Session.GetString("UserId");
            return userId != null ? _db.Users.FirstOrDefault(u => u.Id.ToString() == userId) : null;
        }

        // Get the user by username
        public User? GetUserByCredentials(string username, string password) => _db.Users.FirstOrDefault(u => u.Username == username && u.Password == password); // get the user by username and password

        public string? GetUsername() => GetUser()?.Username; // get the username of the current user

        public string? GetUserId() => GetUser()?.Id.ToString();// get the user id of the current user

        // Get the role of the current user
        public string? GetUserRole()
        {
            return _context?.Session.GetString("Role");
        }

        public void Login(User user)
        {
            _context?.Session.SetString("UserId", user.Id.ToString());
            _context?.Session.SetString("Username", user.Username);
            _context?.Session.SetString("Role", user.Role);
        }

        public void Logout() => _context?.Session.Clear();
    }
}

