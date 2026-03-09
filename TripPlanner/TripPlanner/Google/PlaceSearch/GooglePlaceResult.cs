using TripPlanner.Google.Common;

namespace TripPlanner.Google.PlaceSearch;

public class GooglePlaceResult
{
    public string Name { get; set; } = "";
    public string Formatted_Address { get; set; } = "";
    public string Place_Id { get; set; } = "";

    public Geometry Geometry { get; set; } = new();
}