using Assignment3_ShongChan.Data;
using Assignment3_ShongChan.Models;
using Assignment3_ShongChan.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromSeconds(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddControllersWithViews();
builder.Services.AddControllersWithViews().AddSessionStateTempDataProvider(); 

builder.Services.AddDbContext<AppDbContext>(
    options => options.UseSqlServer(builder.Configuration.GetConnectionString("BookManagementContext")));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Member/Login";
        options.LogoutPath = "/Member/Logout";
    });

// Register the AuthHandler service
builder.Services.AddScoped<AuthHandler>();
builder.Services.AddScoped<BookHandler>();

var app = builder.Build();

//app.MapGet("/", () => "Hello World!");
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession(); // Enable session middleware
app.UseRouting();
app.UseAuthentication(); // Enable authentication middleware
app.UseAuthorization(); // Enable authorization middleware

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
