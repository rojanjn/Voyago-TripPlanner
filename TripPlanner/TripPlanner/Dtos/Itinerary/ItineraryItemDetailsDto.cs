using TripPlanner.Dtos.Location;

namespace TripPlanner.Dtos.Itinerary;

public class ItineraryItemDetailsDto
{
    public int Id { get; set; }

    public int StopOrder { get; set; }

    public DateTime StartDateTime { get; set; }

    public DateTime? EndDateTime { get; set; }

    public LocationDto Location { get; set; } = new();
}