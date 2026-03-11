namespace TripPlanner.Dtos.Itinerary;

public class ItineraryDetailsDto
{
    public int Id { get; set; }

    public string Title { get; set; } = "";

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public List<ItineraryItemDetailsDto> Items { get; set; } = new();
}