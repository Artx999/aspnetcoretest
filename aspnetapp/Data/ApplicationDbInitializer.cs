using assignment_4.Models;
using Microsoft.AspNetCore.Identity;

namespace assignment_4.Data;

public class ApplicationDbInitializer
{
    public static void Initialize(ApplicationDbContext db, UserManager<ApplicationUser> um)
    {
        db.Database.EnsureDeleted();

        db.Database.EnsureCreated();

        // Add data
        var user = new ApplicationUser
            {UserName = "test@test.com", Nickname = "Tester", Email = "test@test.com", EmailConfirmed = true};
        um.CreateAsync(user, "Password123*").Wait();


        var blogEntries = new[]
        {
            new Blog {Title = "A1", Summary = "B1", Content = "C1", ApplicationUser = user},
            new Blog {Title = "A2", Summary = "B2", Content = "C2", ApplicationUser = user, Timestamp = DateTime.Today},
            new Blog {Title = "A3", Summary = "B3", Content = "C3", ApplicationUser = user}
        };
        db.Blogs.AddRange(blogEntries);


        db.SaveChanges();
    }
}