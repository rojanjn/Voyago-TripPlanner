using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TripPlanner.Models;
using TripPlanner.Data;
using Microsoft.EntityFrameworkCore;
using TripPlanner.Dtos.Itinerary;
using TripPlanner.Dtos.Location;
using TripPlanner.Services;


namespace TripPlanner.Controllers
{
    [Authorize]
    public class ItineraryController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly RouteService _routeService;

        public ItineraryController(
            ApplicationDbContext context, 
            RouteService routeService)
        {
            _context = context;
            _routeService = routeService;
        }

        // GET: Itinerary/Index
        // Shows a list of all your itineraries
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (User.IsInRole("Admin"))
            {
                // Admin sees all itineraries
                return View(await _context.Itineraries
                    .Include(i => i.Country)
                    .OrderByDescending(i => i.StartDate)
                    .ToListAsync());
            }

            // Normal users only see their own itineraries
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            return View(await _context.Itineraries
                .Include(i => i.Country)
                .Where(i => i.UserId == userId)
                .OrderByDescending(i => i.StartDate)
                .ToListAsync());
        }

        // GET: Itinerary/Details/{id}
        // Shows a single itinerary page with all its items and locations included
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var itinerary = await _context.Itineraries
                .Include(m => m.Country)
                .Include(m => m.ItineraryItems)
                .ThenInclude(m => m.Location)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (itinerary == null) return NotFound();

            // Non-admins can only view their own itineraries
            if (!User.IsInRole("Admin"))
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (itinerary.UserId != userId) return Forbid();
            }

            return View(itinerary);
        }

        // GET: Itinerary/Create
        // Shows the create form
        // Passes list of countries to the form via ViewBag.Countries
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Countries = _context.Countries
                .OrderBy(c => c.CountryName)
                .ToList();
                return View();
        }

        // POST: Itinerary/Create
        // Saves the new itinerary to the database
        // Automatically assigns UserId to the current logged-in user
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SaveItineraryDto  dto)
        {
            // validate dates
            if (dto.EndDate <= dto.StartDate)
            {
                ModelState.AddModelError("EndDate", "End date must be after start date");
            }

            if (ModelState.IsValid)
            {
                // Map DTO to model, UserId is assigned from the server, never from the form
                var itinerary = new Itinerary
                {
                    Title = dto.Title,
                    Description = dto.Description,
                    StartDate = DateTime.SpecifyKind(dto.StartDate, DateTimeKind.Utc),
                    EndDate = DateTime.SpecifyKind(dto.EndDate, DateTimeKind.Utc),
                    CountryId = dto.CountryId,
                    UserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                };
                
                _context.Add(itinerary);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // if validation failed, reload countries and return to form
            ViewBag.Countries = _context.Countries.OrderBy(c => c.CountryName).ToList();
            return View(dto);
        }

        // GET: Itinerary/Edit/{id}
        // Shows the edit form pre-filled with existing data
        // Also passes countries list for the dropdown
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var itinerary = await _context.Itineraries.FindAsync(id);

            if (itinerary == null) return NotFound();

            // Non-admins can only edit their own itineraries
            if (!User.IsInRole("Admin"))
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (itinerary.UserId != userId) return Forbid();
            }
            
            ViewBag.ItineraryId = itinerary.Id;
            ViewBag.Countries = _context.Countries.OrderBy(c => c.CountryName).ToList();

            // Send the data to view via DTO
            var dto = new SaveItineraryDto
            {
                Title = itinerary.Title,
                Description = itinerary.Description,
                StartDate = itinerary.StartDate,
                EndDate = itinerary.EndDate,
                CountryId = itinerary.CountryId
            };

            return View(dto);
        }

        // POST: Itinerary/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SaveItineraryDto dto)
        {
            // Re-fetch from DB to verify ownership before applying changes
            var existing = await _context.Itineraries.FindAsync(id);
            if (existing == null) return NotFound();

            // Non-admins can only edit their own itineraries
            if (!User.IsInRole("Admin"))
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (existing.UserId != userId) return Forbid();
            }

            // Server-side date validation
            if (dto.EndDate <= dto.StartDate)
                ModelState.AddModelError("EndDate", "End date must be after start date.");

            if (ModelState.IsValid)
            {
                try
                {
                    // Only update allowed fields — UserId is preserved from the existing record
                    existing.Title = dto.Title;
                    existing.Description = dto.Description;
                    existing.StartDate = DateTime.SpecifyKind(dto.StartDate, DateTimeKind.Utc);
                    existing.EndDate = DateTime.SpecifyKind(dto.EndDate, DateTimeKind.Utc);
                    existing.CountryId = dto.CountryId;

                    _context.Update(existing);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    // If the record was deleted by another request, return 404
                    if (!_context.Itineraries.Any(e => e.Id == id))
                        return NotFound();
                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            // Validation failed — reload countries dropdown and return to form with errors
            ViewBag.Countries = _context.Countries.OrderBy(c => c.CountryName).ToList();
            return View(dto);
        }

        // GET: Itinerary/Delete/{id}
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var itinerary = await _context.Itineraries
                .Include(m => m.Country)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (itinerary == null) return NotFound();

            // Non-admins can only delete their own itineraries
            if (!User.IsInRole("Admin"))
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (itinerary.UserId != userId) return Forbid();
            }

            return View(itinerary);
        }

        // POST: Itinerary/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var itinerary = await _context.Itineraries.FindAsync(id);

            if (itinerary == null) return NotFound();

            // Ownership check on POST as well — same reason as Edit POST
            if (!User.IsInRole("Admin"))
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (itinerary.UserId != userId) return Forbid();
            }

            _context.Itineraries.Remove(itinerary);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        
        
        // GET: Itinerary/{id}/DetailsWithItems
        // Returns full itinerary data as JSON including all items and their locations
        [HttpGet("{id}/details")]
        public async Task<IActionResult> GetDetails(int id)
        {
            var itinerary = await _context.Itineraries
                .Include(i => i.ItineraryItems)
                .ThenInclude(item => item.Location)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (itinerary == null)
                return NotFound();

            // ownership check
            if (!User.IsInRole("Admin"))
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                if (itinerary.UserId != userId)
                    return Forbid();
            }

            var result = new ItineraryDetailsDto
            {
                Id = itinerary.Id,
                Title = itinerary.Title,
                StartDate = itinerary.StartDate,
                EndDate = itinerary.EndDate,

                Items = itinerary.ItineraryItems
                    .OrderBy(i => i.StopOrder)
                    .Select(i => new ItineraryItemDetailsDto
                    {
                        Id = i.Id,
                        StopOrder = i.StopOrder,
                        StartDateTime = i.StartDateTime,
                        EndDateTime = i.EndDateTime,

                        Location = new LocationDto
                        {
                            LocationId = i.Location.Id,
                            Name = i.Location.Name,
                            Address = i.Location.Address,
                            Latitude = i.Location.Latitude,
                            Longitude = i.Location.Longitude,
                            Description = i.Location.Description,
                            PlaceId = i.Location.PlaceId
                        }
                    })
                    .ToList()
            };

            return Ok(result);
        }
        
        // Calls RouteService to calculate the full route between all stops
        [HttpGet]
        public async Task<IActionResult> GetRoute(int id)
        {
            var route = await _routeService.GetRoute(id);
                
            if (route == null)
                return NotFound();

            return Ok(route);
        }
    }
}