namespace TripPlanner.Dtos.ItineraryItem;

public class CreateItineraryItemDto
{
    public DateTime StartDateTime { get; set; }
    public DateTime EndDateTime { get; set; }
    public int StopOrder { get; set; }
    public string? Note { get; set; }
}