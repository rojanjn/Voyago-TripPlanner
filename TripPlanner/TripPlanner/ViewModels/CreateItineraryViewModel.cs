using TripPlanner.Models;

namespace TripPlanner.ViewModels;

public class CreateItineraryViewModel
{
    public Itinerary Itinerary { get; set; }

    public IEnumerable<Location> Attractions { get; set; } = new List<Location>();
}