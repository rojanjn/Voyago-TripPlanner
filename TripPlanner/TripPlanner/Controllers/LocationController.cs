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