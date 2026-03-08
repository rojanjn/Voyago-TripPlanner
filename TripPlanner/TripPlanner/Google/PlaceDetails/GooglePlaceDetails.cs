using TripPlanner.Google.Common;

namespace TripPlanner.Google.PlaceDetails;

public class GooglePlaceDetails
{
    public string Name { get; set; } = "";
    public string Formatted_Address { get; set; } = "";
    public string Place_Id { get; set; } = "";
    
    public string? Website { get; set; }
    public string? Formatted_Phone_Number { get; set; }
    
    public double? Rating { get; set; }
    public int? User_Ratings_Total { get; set; }

    public Geometry Geometry { get; set; } = new();
}