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
        
        Console.WriteLine($"[Route] Found {items.Count} items for itinerary {itineraryId}");


        if (items.Count < 2)
        {
            Console.WriteLine("[Route] Not enough items, returning null");
            return null;
        }
            

        var apiKey = _config["GoogleMaps:ApiKey"];
        Console.WriteLine($"[Route] API key present: {!string.IsNullOrEmpty(apiKey)}");


        var origin = $"{items.First().Location.Latitude},{items.First().Location.Longitude}";
        var destination = $"{items.Last().Location.Latitude},{items.Last().Location.Longitude}";
        Console.WriteLine($"[Route] Origin: {origin} | Destination: {destination}");

        
        var waypoints = string.Join("|",
            items.Skip(1).Take(items.Count - 2)
                .Select(i => $"{i.Location.Latitude},{i.Location.Longitude}")
        );

        var url =
            $"https://maps.googleapis.com/maps/api/directions/json?" +
            $"origin={origin}&destination={destination}" +
            $"&waypoints={waypoints}" +
            $"&key={apiKey}";
        
        Console.WriteLine($"[Route] Calling URL: {url}");


        var response = await _httpClient.GetAsync(url);
        Console.WriteLine($"[Route] HTTP status: {response.StatusCode}");


        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine("[Route] Request failed, returning null");

            return null;
        }
            

        var json = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"[Route] Response JSON (first 300 chars): {json[..Math.Min(300, json.Length)]}");


        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var result = JsonSerializer.Deserialize<DirectionsResponse>(json, options);

        if (result == null || result.Routes.Count == 0)
            return null;

        var route = result.Routes.First();
        
        foreach (var leg in route.Legs)
            Console.WriteLine($"RAW: '{leg.Distance.Text}' | '{leg.Duration.Text}'");
        
        Console.WriteLine($"[Route] Routes in response: {result?.Routes?.Count ?? 0}");

        return new RouteDto
        {
            Polyline = route.Overview_Polyline.Points,
    
            TotalDistance = (route.Legs.Sum(l => l.Distance.Value) / 1000.0).ToString("F1") + " km",
    
            // Sum all the legs (Unit: Second, fetch from DirectionsResponse)
            TotalDuration = SumDurations(route.Legs),
    
            // Per-leg duration and distance
            Legs = route.Legs.Select(l => new LegDto
            {
                
                Distance = (l.Distance.Value / 1000.0).ToString("F1") + " km",
                Duration = l.Duration.Text
            }).ToList()
            

        };
        

    }
    

    private double ParseDistance(string text)
    {
        // Remove commas for locale safety (e.g. "1,200 km" → "1200 km")
        text = text.Replace(",", "").Trim();

        if (text.EndsWith(" km"))
        {
            var value = text.Replace(" km", "").Trim();
            return double.TryParse(value, System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out var d) ? d : 0;
        }

        if (text.EndsWith(" m"))
        {
            var value = text.Replace(" m", "").Trim();
            return double.TryParse(value, System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out var d) ? d / 1000.0 : 0;
        }

        return 0;
    }
    
    private string FormatDistanceKm(string text)
    {
        var km = ParseDistance(text); // reuse the fixed parser above
        return $"{km:F1} km";
    }
    
    private string SumDurations(List<Leg> legs)
    {
        var totalSeconds = legs.Sum(l => l.Duration.Value);
        var ts = TimeSpan.FromSeconds(totalSeconds);
    
        if (ts.Hours > 0)
            return $"{ts.Hours} hour {ts.Minutes} mins";
        return $"{ts.Minutes} mins";
    }
}