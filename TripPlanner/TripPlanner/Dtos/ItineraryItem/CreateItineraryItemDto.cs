namespace TripPlanner.Dtos.ItineraryItem;

public class CreateItineraryItemDto
{
    public int LocationId { get; set; }
    public DateTime StartDateTime { get; set; }
    public DateTime EndDateTime { get; set; }
    public int StopOrder { get; set; }
    public string? Note { get; set; }
}