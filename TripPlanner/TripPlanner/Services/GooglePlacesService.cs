using System.Text.Json;
using TripPlanner.Google.PlaceSearch;
using TripPlanner.Google.PlaceDetails;

namespace TripPlanner.Services;

public class GooglePlacesService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;

    public GooglePlacesService(HttpClient httpClient, IConfiguration config)
    {
        _httpClient = httpClient;
        _config = config;
    }

    public async Task<List<GooglePlaceResult>> SearchGooglePlaces(string query)
    {
        var apikey = _config["GoogleMaps:ApiKey"]
            ?? throw new Exception("GoogleMaps ApiKey not configured");
                     
        
        var encodedQuery = Uri.EscapeDataString(query);

        var url = $"https://maps.googleapis.com/maps/api/place/textsearch/json?query={encodedQuery}&key={apikey}";
        
        var response = await _httpClient.GetAsync(url);

        if (!response.IsSuccessStatusCode)
            return new List<GooglePlaceResult>();
        
        var json = await response.Content.ReadAsStringAsync();

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        
        var result = JsonSerializer.Deserialize<GooglePlacesResponse>(json, options);

        return result?.Results ?? new List<GooglePlaceResult>();
    }
    
    public async Task<GooglePlaceDetails?> GetPlaceDetailsAsync(string placeId)
    {
        var apiKey = _config["GoogleMaps:ApiKey"];
    
        var url =
            $"https://maps.googleapis.com/maps/api/place/details/json?place_id={placeId}&key={apiKey}";

        var response = await _httpClient.GetAsync(url);

        if (!response.IsSuccessStatusCode) return null;

        var json = await response.Content.ReadAsStringAsync();

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
         
        var result = JsonSerializer.Deserialize<GooglePlaceDetailsResponse>(json, options);
        
        return result?.Result;
    }
}


