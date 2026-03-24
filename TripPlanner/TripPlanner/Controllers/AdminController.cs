using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TripPlanner.Data;
using TripPlanner.Models;

namespace TripPlanner.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        // Constructor to inject the database context
        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Action to display admin dashboard with statistics
        public async Task<IActionResult> Index()
        {
            var now = DateTime.UtcNow; // Use UTC for PostgreSQL

            var stats = new
            {
                TotalUsers = await _context.Users.CountAsync(),
                TotalItineraries = await _context.Itineraries.CountAsync(),
                ActiveItineraries = await _context.Itineraries.CountAsync(i => i.EndDate.ToUniversalTime() >= now),
                PastItineraries = await _context.Itineraries.CountAsync(i => i.EndDate.ToUniversalTime() < now),
                TotalCountries = await _context.Countries.CountAsync(),
                TotalLocations = await _context.Locations.CountAsync(),
                ItinerariesThisMonth = await _context.Itineraries
                    .CountAsync(i => i.StartDate.Month == now.Month && i.StartDate.Year == now.Year)
            };

            return View(stats);
        }

        // Action to display a list of all users with their details and itinerary counts
        public async Task<IActionResult> Users()
        {
            var users = await _context.Users
                .Select(u => new {
                    u.Email,
                    u.UserName,
                    ItineraryCount = u.Itineraries.Count,
                    ActiveTrips = u.Itineraries.Count(i => i.EndDate >= DateTime.UtcNow)
                })
                .OrderBy(u => u.Email)
                .ToListAsync();

            return View(users);
        }

        // Action to display a list of all itineraries with details and options to manage them
        public async Task<IActionResult> AllItineraries()
        {
            // Include related Country and User data for display purposes
            var itineraries = await _context.Itineraries
                .Include(i => i.Country)
                .Include(i => i.User)
                .OrderByDescending(i => i.StartDate)
                .ToListAsync();

            return View(itineraries);
        }

        // Action to delete an itinerary
        [HttpPost]
        public async Task<IActionResult> DeleteItinerary(int id)
        {
            // Find the itinerary by ID and delete it if it exists
            var itinerary = await _context.Itineraries.FindAsync(id);
            if (itinerary != null)
            {
                _context.Itineraries.Remove(itinerary);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(AllItineraries));
        }
    }
}