using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TripPlanner.Models;
using TripPlanner.Data;
using Microsoft.EntityFrameworkCore;
using TripPlanner.Dtos.Itinerary;
using TripPlanner.Dtos.ItineraryItem;
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
        public async Task<IActionResult> Create(Itinerary itinerary)
        {
            if (ModelState.IsValid)
            {
                // Assign the current user as the owner before saving
                itinerary.UserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                
                itinerary.StartDate = DateTime.SpecifyKind(itinerary.StartDate, DateTimeKind.Utc);
                itinerary.EndDate = DateTime.SpecifyKind(itinerary.EndDate, DateTimeKind.Utc);

                _context.Add(itinerary);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(itinerary);
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

            ViewBag.Countries = _context.Countries.OrderBy(c => c.CountryName).ToList();
            return View(itinerary);
        }

        // POST: Itinerary/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Itinerary itinerary)
        {
            // The id in the URL must match the itinerary being submitted
            if (id != itinerary.Id) return NotFound();

            // Re-fetch from DB to verify ownership before allowing the edit
            // We do this on POST as well to prevent someone crafting a direct POST request
            var existing = await _context.Itineraries.FindAsync(id);

            if (existing == null) return NotFound();

            if (!User.IsInRole("Admin"))
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                // Block the edit if the itinerary doesn't belong to this user
                if (existing.UserId != userId) return Forbid();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Only update the fields users are allowed to change
                    // UserId is preserved from the existing record, not taken from the form
                    existing.Title = itinerary.Title;
                    existing.Description = itinerary.Description;
                    existing.StartDate = itinerary.StartDate;
                    existing.EndDate = itinerary.EndDate;
                    existing.CountryId = itinerary.CountryId;

                    existing.StartDate = DateTime.SpecifyKind(itinerary.StartDate, DateTimeKind.Utc);
                    existing.EndDate = DateTime.SpecifyKind(itinerary.EndDate, DateTimeKind.Utc);

                    _context.Update(existing);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    // If the record no longer exists, return 404
                    if (!_context.Itineraries.Any(e => e.Id == id))
                        return NotFound();

                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            return View(itinerary);
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

        [HttpPost]
        public async Task<IActionResult> AddAttraction([FromBody] AddAttractionDto dto)
        {
            var itinerary = await _context.Itineraries.FindAsync(dto.ItineraryId);
            if (itinerary == null) return NotFound();

            // Ownership check to match your existing pattern
            if (!User.IsInRole("Admin"))
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (itinerary.UserId != userId) return Forbid();
            }
            
            // Check if location already exists by PlaceId
            var location = await _context.Locations
                .FirstOrDefaultAsync(l => l.PlaceId == dto.PlaceId);
            
            // If not, create it
            if (location == null)
            {
                location = new Location
                {
                    Name = dto.Name,
                    Address = dto.Address,
                    Latitude = dto.Latitude,
                    Longitude = dto.Longitude,
                    PlaceId = dto.PlaceId
                };
                _context.Locations.Add(location);
                await _context.SaveChangesAsync();
            }

            var nextStopOrder = await _context.ItineraryItems
                .Where(i => i.ItineraryId == dto.ItineraryId)
                .MaxAsync(i => (int?)i.StopOrder) ?? 0;

            var item = new ItineraryItem
            {
                ItineraryId = dto.ItineraryId,
                LocationId = location.Id,
                StopOrder = nextStopOrder + 1,
                StartDateTime = DateTime.SpecifyKind(itinerary.StartDate, DateTimeKind.Utc),
                EndDateTime = DateTime.SpecifyKind(itinerary.StartDate.AddHours(1), DateTimeKind.Utc)
            };

            _context.ItineraryItems.Add(item);
            await _context.SaveChangesAsync();

            return Ok();
        }
        
        [HttpPost]
        public async Task<IActionResult> RemoveAttraction([FromBody] RemoveAttractionDto dto)
        {
            var item = await _context.ItineraryItems.FindAsync(dto.ItineraryItemId);
            if (item == null) return NotFound();
            
            // Ownership check
            var itinerary = await _context.Itineraries.FindAsync(item.ItineraryId);
            if (itinerary == null) return NotFound();
            if (!User.IsInRole("Admin"))
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (itinerary.UserId != userId) return Forbid();
            }

            _context.ItineraryItems.Remove(item);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> ReorderAttraction([FromBody] ReorderAttractionsDto dto)
        {
            Console.WriteLine($"ReorderAttraction called with itineraryId: {dto.ItineraryId}");
            Console.WriteLine($"ItemIds: {string.Join(", ", dto.ItemIds)}");
            
            var itinerary = await _context.Itineraries.FindAsync(dto.ItineraryId);
            if (itinerary == null) return NotFound();

            if (!User.IsInRole("Admin"))
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (itinerary.UserId != userId) return Forbid();
            }

            for (int i = 0; i < dto.ItemIds.Count; i++)
            {
                var item = await _context.ItineraryItems.FindAsync(dto.ItemIds[i]);
                Console.WriteLine($"Item {dto.ItemIds[i]}: StopOrder changing to {i + 1}");
                if (item == null) return NotFound();
                item.StopOrder = i + 1;
            }
            
            await _context.SaveChangesAsync();
            Console.WriteLine("SaveChanges complete");
            
            return Ok();
        }
    }
}