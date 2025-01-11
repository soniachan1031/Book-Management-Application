using Assignment3_ShongChan.Models;
using Assignment3_ShongChan.Models.DomainModels;

namespace Assignment3_ShongChan.Models.ViewModels
{
    public class SearchUserViewModel
    {
        public List<User>? Users { get; set; } = new List<User>();
        public string? SearchTerm { get; set; }
        public string? SortBy { get; set; }
        public string? SortOrder { get; set; }
        public bool? HasBookAssigned { get; set; }
    }
}
