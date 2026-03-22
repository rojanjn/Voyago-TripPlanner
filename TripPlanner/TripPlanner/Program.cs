using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TripPlanner.Data;
using TripPlanner.Models;
using TripPlanner.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
                       throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("TripPlanner")));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Add services for controllers with views. Necessary for MVC functionality in the application.
builder.Services.AddControllersWithViews();

// Add HttpClient service
builder.Services.AddHttpClient<GooglePlacesService>();
builder.Services.AddHttpClient<RouteService>();

// Set False for now
builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
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
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages()
    .WithStaticAssets();



// ---------------------------------------------------
// ROLE + ADMIN SEEDING
// This section is responsible for initializing default roles and creating an initial admin user
// if they do not already exist in the database. This is a common practice for setting up
// initial security configurations in ASP.NET Core Identity applications.
// ---------------------------------------------------
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    
    // Retrieve RoleManager and UserManager services from the service provider.
    // These services are essential for interacting with the Identity system for roles and users.
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    

    // Define the application's roles. These roles will be created if they don't already exist.
    string[] roles = { "Admin", "User" };

    // Iterate through each defined role.
    foreach (var role in roles)
    {
        // Check if the role already exists. If not, create it.
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));
    }

    // Create default admin user
    // This section attempts to find an admin user by a predefined email.
    // If no admin user is found, a new one is created and assigned the "Admin" role.
    var adminEmail = "admin@admin.com";
    var admin = await userManager.FindByEmailAsync(adminEmail);

    // If the admin user does not exist, create a new IdentityUser instance.
    if (admin == null)
    {
        admin = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true // Mark email as confirmed to allow immediate login.
        };

        // Create the admin user with a specified password.
        await userManager.CreateAsync(admin, "P@ssw0rd");
        // Assign the newly created user to the "Admin" role.
        await userManager.AddToRoleAsync(admin, "Admin");
    }

    // Create seed data for itinerary after the admin had been created 
    if (!db.Itineraries.Any())
    {
        var trip1 = new Itinerary
        {
            UserId = admin.Id,
            CountryId = 1,
            Title = "First Trip",
            StartDate = new DateTime(2026, 1, 15, 9, 0, 0, DateTimeKind.Utc),
            EndDate = new DateTime(2026, 3, 14, 9, 0, 0, DateTimeKind.Utc)
        };
        var trip2 = new Itinerary
        {
            UserId = admin.Id,
            CountryId = 2,
            Title = "Second Trip",
            StartDate = new DateTime(2026, 3, 15, 9, 0, 0, DateTimeKind.Utc),
            EndDate = new DateTime(2026, 6, 14, 9, 0, 0, DateTimeKind.Utc)
        };
        
        db.Itineraries.AddRange(trip1, trip2);
        await db.SaveChangesAsync();
        
        db.ItineraryItems.AddRange(
            new ItineraryItem { 
                ItineraryId = trip1.Id,
                LocationId = 1,
                StartDateTime = new DateTime(2026, 1, 16, 9, 0, 0, DateTimeKind.Utc),
                EndDateTime = new DateTime(2026, 2, 15, 9, 0, 0, DateTimeKind.Utc),
                StopOrder = 1
            },
            new ItineraryItem { 
                ItineraryId = trip1.Id,
                LocationId = 2,
                StartDateTime = new DateTime(2026, 2, 16, 9, 0, 0, DateTimeKind.Utc),
                EndDateTime = new DateTime(2026, 3, 15, 9, 0, 0, DateTimeKind.Utc),
                StopOrder = 2
            },
            new ItineraryItem { 
                ItineraryId = trip2.Id,
                LocationId = 3,
                StartDateTime = new DateTime(2026, 3, 16, 9, 0, 0, DateTimeKind.Utc),
                EndDateTime = new DateTime(2026, 4, 15, 9, 0, 0, DateTimeKind.Utc),
                StopOrder = 1
            },
            new ItineraryItem { 
                ItineraryId = trip2.Id,
                LocationId = 4,
                StartDateTime = new DateTime(2026, 4, 16, 9, 0, 0, DateTimeKind.Utc),
                EndDateTime = new DateTime(2026, 5, 15, 9, 0, 0, DateTimeKind.Utc),
                StopOrder = 2
            });
    }
}

app.Run();