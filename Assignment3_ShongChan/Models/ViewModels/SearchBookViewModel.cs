using Assignment3_ShongChan.Models;
using Assignment3_ShongChan.Models.DomainModels;

namespace Assignment3_ShongChan.Models.ViewModels
{
    public class SearchBookViewModel
    {
        public List<Book>? Books { get; set; }
        public List<Genre> Genres { get; set; } = new List<Genre>();
        public string? SearchTerm { get; set; }
        public string? GenreFilter { get; set; }
        public string? AvailabilityFilter { get; set; }
        public string? SortOrder { get; set; }
        public string? SortBy { get; set; }
        public bool? Available { get; set; }
        public string? SelectedGenre { get; set; }

        public List<string> GenreNames 
            => Genres.Where(g => !string
            .IsNullOrEmpty(g.Name))
            .Select(g => g.Name).Distinct().ToList(); 
    }
}
