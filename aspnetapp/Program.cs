using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using assignment_4.Data;
using assignment_4.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

var app = builder.Build();

using (var services = app.Services.CreateScope())
{
    // Get our database context from the service provider
    var db = services.ServiceProvider.GetRequiredService<ApplicationDbContext>();
  
    // Get the UserManager and RoleManager also from the service provider
    var um = services.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    // Initialise the database using the initializer from Data/ExampleDbInitializer.cs
    if (app.Environment.IsDevelopment())
        ApplicationDbInitializer.Initialize(db, um);
    else
        db.Database.Migrate();
}


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();