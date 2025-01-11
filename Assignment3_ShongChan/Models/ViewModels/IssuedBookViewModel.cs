using Assignment3_ShongChan.Models.DomainModels;

namespace Assignment3_ShongChan.Models.ViewModels
{
    public class IssuedBookViewModel
    {
        public string MemberName { get; set; } = string.Empty;
        public string BookTitle { get; set; }
        public Book? IssuedBook { get; set; }
        public int MemberId { get; set; }
        public int BookId { get; set; }
        public IEnumerable<User> Members { get; set; } = new List<User>();
        public IEnumerable<Book> Books { get; set; } = new List<Book>();
    }
}
