using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TripPlanner.Models;
using TripPlanner.Data;
using TripPlanner.Dtos.Location;
using TripPlanner.Services;

namespace TripPlanner.Controllers;

[ApiController]
[Route("locations")]
[Authorize]
public class LocationController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly GooglePlacesService _googleService;

    public LocationController(ApplicationDbContext context, GooglePlacesService googleService)
    {
        _context = context;
        _googleService = googleService;
    }
        
    /*
    // Get /locations
    // Returns all locations currently saved in the DB
    [HttpGet]
    public async Task<IActionResult> GetAll()
    { 
        var locations = await _context.Locations.ToListAsync();
        var dto = locations.Select(l => new LocationDto
        {
            LocationId = l.Id,
            Name = l.Name,
            Address = l.Address,
            Latitude = l.Latitude,
            Longitude = l.Longitude,
            Description = l.Description,
            PlaceId = l.PlaceId
        });
        return Ok(dto);
    }
        
        
    // Get  /locations/{id}
    // Returns a single location by its DB ID (To view its details or update the location info)
    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var location = await _context.Locations.FindAsync(id);
        if (location == null) return NotFound();

        var dto = new LocationDto
        {
            LocationId = location.Id,
            Name = location.Name,
            Address = location.Address,
            Latitude = location.Latitude,
            Longitude = location.Longitude,
            Description = location.Description,
            PlaceId = location.PlaceId
        };
        return Ok(dto);
    }
        
        
    // POST /locations
    // Saves a new location, which is originates from Google, into your database
    [HttpPost]
    public async Task<IActionResult> Create(CreateLocationDto dto)
    {
        var location = new Location
        {
            Name = dto.Name,
            Address = dto.Address,
            Latitude = dto.Latitude,
            Longitude = dto.Longitude,
            Description = dto.Description,
            PlaceId = dto.PlaceId
        };
            
        _context.Locations.Add(location);
        await _context.SaveChangesAsync();
            
        return Ok(new LocationDto
        {
            LocationId = location.Id,
            Name = location.Name,
            Address = location.Address,
            Latitude = location.Latitude,
            Longitude = location.Longitude,
            Description = location.Description,
            PlaceId = location.PlaceId
        });
    }
    
    
        
    // PUT /locations/{id}
    // Updates an existing location's details
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateLocationDto dto)
    {
        var location = await _context.Locations.FindAsync(id);
        if (location == null) return NotFound();

        location.Name = dto.Name;
        location.Address = dto.Address;
        location.Latitude = dto.Latitude;
        location.Longitude = dto.Longitude;
        location.Description = dto.Description;
        location.PlaceId = dto.PlaceId;

        await _context.SaveChangesAsync();
        return NoContent();
    }

        
        
    // DELETE /locations/{id}
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var location = await _context.Locations.FindAsync(id);
        if (location == null) return NotFound();

        // To ensure no ItineraryItem using this Location
        var used = await _context.ItineraryItems.AnyAsync(i => i.LocationId == id);
        if (used) return BadRequest("Location is used by an itinerary item.");

        _context.Locations.Remove(location);
        await _context.SaveChangesAsync();

        return NoContent();
    }
    
    
        
    // GET /locations/search?query=toronto
    [HttpGet("search")]
    public async Task<IActionResult> Search(string query)
    {
        // A service that wraps Google Places API.
        // SearchGooglePlaces(query) sends the search request and returns a list of place results.
        var places = await _googleService.SearchGooglePlaces(query);

        // The Select projects each place into an anonymous object that contains only the fields you want the client to see.
        var results = places.Select(p => new
        {
            name = p.Name,
            address = p.Formatted_Address,
            latitude = p.Geometry.Location.Lat,
            longitude = p.Geometry.Location.Lng,
            placeId = p.Place_Id
        });

        return Ok(results);
        
        
    }
    */
        
    // GET /Itinerary/SearchAttractions?query=
    // Searches locations already in your database by name
    [HttpGet]
    public async Task<IActionResult> SearchLocations(string query)
    {
        var results = await _context.Locations
            .Where(l => l.Name.Contains(query))
            .Select(l => new { l.Id, l.Name, l.Address })
            .ToListAsync();

        return Ok(results);
    }
        
        
        
        
        
        
        
    
}