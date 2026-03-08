namespace TripPlanner.Dtos.ItineraryItem;

public class UpdateItineraryItemDto
{
    public int LocationId { get; set; }
    public DateTime StartDateTime { get; set; }
    public DateTime EndDateTime { get; set; }
    public int StopOrder { get; set; }
    public string? Note { get; set; }
}