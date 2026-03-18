namespace TripPlanner.Dtos.Itinerary;

public class ReorderAttractionsDto
{
    public int ItineraryId { get; set; }
    public List<int> ItemIds { get; set; } = new();
}