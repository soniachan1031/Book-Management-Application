using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Assignment3_ShongChan.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Genres",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Genres", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Images",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImagePath = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Images", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Books",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Author = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    GenreId = table.Column<int>(type: "int", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    AvailableQuantity = table.Column<int>(type: "int", nullable: false),
                    IsAvailable = table.Column<bool>(type: "bit", nullable: false),
                    BorrowedById = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Books", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Books_Genres_GenreId",
                        column: x => x.GenreId,
                        principalTable: "Genres",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Age = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProfilePic = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProfileImageId = table.Column<int>(type: "int", nullable: true),
                    BookId = table.Column<int>(type: "int", nullable: true),
                    IssuedBookId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Books_BookId",
                        column: x => x.BookId,
                        principalTable: "Books",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Users_Images_ProfileImageId",
                        column: x => x.ProfileImageId,
                        principalTable: "Images",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "Genres",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Fiction" },
                    { 2, "Non-Fiction" },
                    { 3, "Mystery" },
                    { 4, "Fantasy" },
                    { 5, "Science" }
                });

            migrationBuilder.InsertData(
                table: "Images",
                columns: new[] { "Id", "ContentType", "ImagePath", "Name" },
                values: new object[] { 1, "image/jpeg", "/Images/default-profile.png", "default" });

            migrationBuilder.InsertData(
                table: "Books",
                columns: new[] { "Id", "Author", "AvailableQuantity", "BorrowedById", "GenreId", "IsAvailable", "Quantity", "Title", "Year" },
                values: new object[,]
                {
                    { 1, "F. Scott Fitzgerald", 0, null, 1, false, 5, "The Great Gatsby", 1925 },
                    { 2, "George Orwell", 2, null, 3, true, 8, "1984", 1949 },
                    { 3, "Yuval Noah Harari", 2, null, 2, true, 5, "Sapiens: A Brief History of Humankind", 2011 },
                    { 4, "Stieg Larsson", 1, null, 3, true, 3, "The Girl with the Dragon Tattoo", 2005 },
                    { 5, "Aldous Huxley", 4, null, 5, true, 7, "Brave New World", 1932 }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Age", "BookId", "DateOfBirth", "IssuedBookId", "Name", "Password", "ProfileImageId", "ProfilePic", "Role", "Username" },
                values: new object[,]
                {
                    { 1, "30", null, new DateTime(1993, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Admin", "Admin123!", 1, "/Images/default-profile.png", "Admin", "admin" },
                    { 5, "27", null, new DateTime(1996, 9, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "David Williams", "David@123", 1, "/Images/default-profile.png", "Member", "David_williams" },
                    { 6, "24", null, new DateTime(1999, 11, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Eva Davis", "Eva@123", 1, "/Images/default-profile.png", "Member", "Eva_davis" },
                    { 2, "25", 1, new DateTime(1998, 5, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Member", "Member123!", 1, "/Images/default-profile.png", "Member", "member" },
                    { 3, "28", 2, new DateTime(1995, 8, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Bob Smith", "Bob@123", 1, "/Images/default-profile.png", "Member", "Bob_smith" },
                    { 4, "35", 3, new DateTime(1988, 3, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Charlie Brown", "Charlie@123", 1, "/Images/default-profile.png", "Member", "Charlie_brown" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Books_BorrowedById",
                table: "Books",
                column: "BorrowedById");

            migrationBuilder.CreateIndex(
                name: "IX_Books_GenreId",
                table: "Books",
                column: "GenreId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_BookId",
                table: "Users",
                column: "BookId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_ProfileImageId",
                table: "Users",
                column: "ProfileImageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Books_Users_BorrowedById",
                table: "Books",
                column: "BorrowedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Books_Genres_GenreId",
                table: "Books");

            migrationBuilder.DropForeignKey(
                name: "FK_Books_Users_BorrowedById",
                table: "Books");

            migrationBuilder.DropTable(
                name: "Genres");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Books");

            migrationBuilder.DropTable(
                name: "Images");
        }
    }
}
