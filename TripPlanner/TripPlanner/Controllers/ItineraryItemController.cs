using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TripPlanner.Data;
using TripPlanner.Dtos.ItineraryItem;
using TripPlanner.Models;



namespace TripPlanner.Controllers;

[Authorize]
[ApiController]
[Route("itineraries/{itineraryId:int}/items")]
public class ItineraryItemController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    // Ownership Verification Core
    public ItineraryItemController(
        // Get Itineraries, ItineraryItems
        ApplicationDbContext context,
        
        // Get User ID from sign in status
        UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }
    
    
    
    // Get Current User Id
    private string GetCurrentUserId() => _userManager.GetUserId(User);
    
    // Itinerary Ownership Check
    private async Task<Itinerary?> GetOwnedItineraryAsync(int itineraryId)
    {
        // Fetch the itinerary ID
        var itinerary = await _context.Itineraries
            .FirstOrDefaultAsync(i => i.Id == itineraryId);

        if (itinerary == null) return null;

        // Full permission for admin
        if (User.IsInRole("Admin")) return itinerary;

        // Get user's own itineraries
        var userId = GetCurrentUserId();
        if (itinerary.UserId == userId) return itinerary;

        return null;
    }
    
    
    
    // Read Items (GET)
    // Returns all items in a specific itinerary
    [HttpGet]
    public async Task<IActionResult> GetItems(int itineraryId)
    {
        var itinerary = await GetOwnedItineraryAsync(itineraryId);
        if (itinerary == null) return NotFound();

        var items = await _context.ItineraryItems
            .Include(i => i.Location)
            .Where(i => i.ItineraryId == itineraryId)
            .OrderBy(i => i.StopOrder)
            .ToListAsync();

        var itemsDto = items.Select(i => new ItineraryItemDto
        {
            Id = i.Id,
            LocationId = i.LocationId,
            LocationName = i.Location.Name,
            StartDateTime = i.StartDateTime,
            EndDateTime = i.EndDateTime,
            StopOrder = i.StopOrder,
            Note = i.Note
        });

        return Ok(itemsDto);
    }
    
    
    
    // POST /itineraries/{itineraryId}/items
    // Saves location if it doesn't exist yet, then adds it as a stop
    [HttpPost]
    public async Task<IActionResult> CreateItem(int itineraryId, CreateItineraryItemDto dto)
    {
        var itinerary = await GetOwnedItineraryAsync(itineraryId);
        if (itinerary == null) return NotFound();

        // Check if location already exists by PlaceId, create it if not
        var location = await _context.Locations
            .FirstOrDefaultAsync(l => l.PlaceId == dto.Location.PlaceId);

        if (location == null)
        {
            location = new Location
            {
                Name = dto.Location.Name,
                Address = dto.Location.Address,
                Latitude = dto.Location.Latitude,
                Longitude = dto.Location.Longitude,
                PlaceId = dto.Location.PlaceId
            };
            _context.Locations.Add(location);
            await _context.SaveChangesAsync();
        }

        var nextStopOrder = await _context.ItineraryItems
            .Where(i => i.ItineraryId == itineraryId)
            .MaxAsync(i => (int?)i.StopOrder) ?? 0;

        var item = new ItineraryItem
        {
            ItineraryId = itineraryId,
            LocationId = location.Id,
            StopOrder = nextStopOrder + 1,
            StartDateTime = DateTime.SpecifyKind(itinerary.StartDate, DateTimeKind.Utc),
            EndDateTime = DateTime.SpecifyKind(itinerary.StartDate.AddHours(1), DateTimeKind.Utc),
            Note = dto.Note
        };

        _context.ItineraryItems.Add(item);
        await _context.SaveChangesAsync();

        var resultDto = new ItineraryItemDto
        {
            Id = item.Id,
            LocationId = item.LocationId,
            LocationName = location.Name,
            StartDateTime = item.StartDateTime,
            EndDateTime = item.EndDateTime,
            StopOrder = item.StopOrder,
            Note = item.Note
        };

        return Ok(resultDto);
    }

    // PUT /itineraries/{itineraryId}/items/{itemId}
    // Updates an existing stop's time, order, and note
    [HttpPut("{itemId:int}")]
    public async Task<IActionResult> UpdateItem(
        int itineraryId,
        int itemId,
        UpdateItineraryItemDto dto)
    {
        var itinerary = await GetOwnedItineraryAsync(itineraryId);
        if (itinerary == null) return NotFound();

        var item = await _context.ItineraryItems
            .FirstOrDefaultAsync(i => i.Id == itemId && i.ItineraryId == itineraryId);

        if (item == null) return NotFound();

        item.StartDateTime = DateTime.SpecifyKind(dto.StartDateTime,  DateTimeKind.Utc);
        item.EndDateTime = DateTime.SpecifyKind(dto.EndDateTime,  DateTimeKind.Utc);
        item.StopOrder = dto.StopOrder;
        item.Note = dto.Note;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    // PUT /itineraries/{itineraryId}/items/reorder
    // Updates StopOrder for all items based on the provided ordered list of IDs
    [HttpPut("reorder")]
    public async Task<IActionResult> ReorderItems(int itineraryId, [FromBody] List<int> itemIds)
    {
        var itinerary = await GetOwnedItineraryAsync(itineraryId);
        if (itinerary == null) return NotFound();

        for (int i = 0; i < itemIds.Count; i++)
        {
            var item = await _context.ItineraryItems.FindAsync(itemIds[i]);
            if (item == null) return NotFound();
            item.StopOrder = i + 1;
            _context.Entry(item).Property(x => x.StopOrder).IsModified = true;
        }

        await _context.SaveChangesAsync();
        return NoContent();
    }

    // DELETE /itineraries/{itineraryId}/items/{itemId}
    // Removes a stop from the itinerary
    [HttpDelete("{itemId:int}")]
    public async Task<IActionResult> DeleteItem(int itineraryId, int itemId)
    {
        var itinerary = await GetOwnedItineraryAsync(itineraryId);
        if (itinerary == null) return NotFound();

        var item = await _context.ItineraryItems
            .FirstOrDefaultAsync(i => i.Id == itemId && i.ItineraryId == itineraryId);

        if (item == null) return NotFound();

        _context.ItineraryItems.Remove(item);
        await _context.SaveChangesAsync();
        return NoContent();
    }
    
    
    
}


