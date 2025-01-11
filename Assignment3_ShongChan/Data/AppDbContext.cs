using Microsoft.EntityFrameworkCore;
using Assignment3_ShongChan.Models.DomainModels;

namespace Assignment3_ShongChan.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Image> Images { get; set; } = null!;
        public DbSet<Book> Books { get; set; } = null!;
        public DbSet<Genre> Genres { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Genre>().HasData(
                new Genre { Id = 1, Name = "Fiction" },
                new Genre { Id = 2, Name = "Non-Fiction" },
                new Genre { Id = 3, Name = "Mystery" },
                new Genre { Id = 4, Name = "Fantasy" },
                new Genre { Id = 5, Name = "Science" }
            );

            modelBuilder.Entity<Book>().HasData(
                new Book { Id = 1, Title = "The Great Gatsby", Author = "F. Scott Fitzgerald", GenreId = 1, Year = 1925, Quantity = 5, AvailableQuantity = 0, IsAvailable = false },
                new Book { Id = 2, Title = "1984", Author = "George Orwell", GenreId = 3, Year = 1949, Quantity = 8, AvailableQuantity = 2, IsAvailable = true },
                new Book { Id = 3, Title = "Sapiens: A Brief History of Humankind", Author = "Yuval Noah Harari", GenreId = 2, Year = 2011, Quantity = 5, AvailableQuantity = 2, IsAvailable = true },
                new Book { Id = 4, Title = "The Girl with the Dragon Tattoo", Author = "Stieg Larsson", GenreId = 3, Year = 2005, Quantity = 3, AvailableQuantity = 1, IsAvailable = true },
                new Book { Id = 5, Title = "Brave New World", Author = "Aldous Huxley", GenreId = 5, Year = 1932, Quantity = 7, AvailableQuantity = 4, IsAvailable = true }
            );

            modelBuilder.Entity<Image>().HasData(
                new Image
                {
                    Id = 1,
                    Name = "default",
                    ContentType = "image/jpeg",
                    ImagePath = "/Images/default-profile.png"
                }
            );

            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Username = "admin",
                    Password = "Admin123!",
                    ConfirmPassword = "Admin123!",
                    Name = "Admin",
                    DateOfBirth = new DateTime(1993, 1, 1),
                    Age = "30",
                    Role = "Admin",
                    ProfilePic = "/Images/default-profile.png", 
                    ProfileImageId = 1,
                    BookId = null
                },
                new User
                {
                    Id = 2,
                    Username = "member",
                    Password = "Member123!",
                    ConfirmPassword = "Member123!",
                    Name = "Member",
                    DateOfBirth = new DateTime(1998, 5, 20),
                    Age = "25",
                    Role = "Member",
                    ProfilePic = "/Images/default-profile.png",
                    ProfileImageId = 1,
                    BookId = 1
                }, 
                new User
                {
                    Id = 3,
                    Username = "Bob_smith",
                    Password = "Bob@123",
                    ConfirmPassword = "Bob@123",
                    Name = "Bob Smith",
                    DateOfBirth = new DateTime(1995, 8, 20),
                    Age = "28",
                    Role = "Member",
                    ProfilePic = "/Images/default-profile.png",
                    ProfileImageId = 1,
                    BookId = 2
                },
                new User
                {
                    Id = 4,
                    Username = "Charlie_brown",
                    Password = "Charlie@123",
                    ConfirmPassword = "Charlie@123",
                    Name = "Charlie Brown",
                    DateOfBirth = new DateTime(1988, 3, 20),
                    Age = "35",
                    Role = "Member",
                    ProfilePic = "/Images/default-profile.png",
                    ProfileImageId = 1,
                    BookId = 3
                },
                new User
                {
                    Id = 5,
                    Username = "David_williams",
                    Password = "David@123",
                    ConfirmPassword = "David@123",
                    Name = "David Williams",
                    DateOfBirth = new DateTime(1996, 9, 10),
                    Age = "27",
                    Role = "Member",
                    ProfilePic = "/Images/default-profile.png",
                    ProfileImageId = 1,
                    BookId = null
                },
                new User
                {
                    Id = 6,
                    Username = "Eva_davis",
                    Password = "Eva@123",
                    ConfirmPassword = "Eva@123",
                    Name = "Eva Davis",
                    DateOfBirth = new DateTime(1999, 11, 25),
                    Age = "24",
                    Role = "Member",
                    ProfilePic = "/Images/default-profile.png",
                    ProfileImageId = 1,
                    BookId = null
                }
            );

            // Set up relationships
            modelBuilder.Entity<Genre>().HasKey(g => g.Id);
            modelBuilder.Entity<Book>().HasKey(b => b.Id);
            modelBuilder.Entity<Image>().HasKey(i => i.Id);
            modelBuilder.Entity<User>().HasKey(u => u.Id);

            // Book to User (BorrowedBy relationship)
            modelBuilder.Entity<Book>()
                .HasOne(b => b.BorrowedBy)
                .WithMany(u => u.IssuedBooks)
                .HasForeignKey(b => b.BorrowedById)
                .OnDelete(DeleteBehavior.Restrict);

            // User to Book (Assigned book relationship by admin)
            modelBuilder.Entity<User>()
                .HasOne(u => u.Book)
                .WithMany()
                .HasForeignKey(u => u.BookId)
                .OnDelete(DeleteBehavior.Restrict);

            // Avoid duplicate relationships
            modelBuilder.Entity<User>().Ignore(u => u.IssuedBook);
        }
    }
}