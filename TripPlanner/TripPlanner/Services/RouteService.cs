using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using TripPlanner.Data;
using TripPlanner.Dtos.Route;
using TripPlanner.Google.Directions;

namespace TripPlanner.Services;

public class RouteService
{
    private readonly ApplicationDbContext _context;
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;

    public RouteService(
        ApplicationDbContext context,
        HttpClient httpClient,
        IConfiguration config)
    {
        _context = context;
        _httpClient = httpClient;
        _config = config;
    }

    public async Task<RouteDto?> GetRoute(int itineraryId)
    {
        var items = await _context.ItineraryItems
            .Include(i => i.Location)
            .Where(i => i.ItineraryId == itineraryId)
            .OrderBy(i => i.StopOrder)
            .ToListAsync();

        if (items.Count < 2)
            return null;

        var apiKey = _config["GoogleMaps:ApiKey"];

        var origin = $"{items.First().Location.Latitude},{items.First().Location.Longitude}";
        var destination = $"{items.Last().Location.Latitude},{items.Last().Location.Longitude}";

        var waypoints = string.Join("|",
            items.Skip(1).Take(items.Count - 2)
                .Select(i => $"{i.Location.Latitude},{i.Location.Longitude}")
        );

        var url =
            $"https://maps.googleapis.com/maps/api/directions/json?" +
            $"origin={origin}&destination={destination}" +
            $"&waypoints={waypoints}" +
            $"&key={apiKey}";

        var response = await _httpClient.GetAsync(url);

        if (!response.IsSuccessStatusCode)
            return null;

        var json = await response.Content.ReadAsStringAsync();

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var result = JsonSerializer.Deserialize<DirectionsResponse>(json, options);

        if (result == null || result.Routes.Count == 0)
            return null;

        var route = result.Routes.First();

        return new RouteDto
        {
            Polyline = route.Overview_Polyline.Points,
            Distance = route.Legs.Sum(l => ParseDistance(l.Distance.Text)).ToString() + " km",
            Duration = route.Legs.First().Duration.Text
        };
    }

    private double ParseDistance(string text)
    {
        var value = text.Replace(" km", "");
        return double.TryParse(value, out var d) ? d : 0;
    }
}