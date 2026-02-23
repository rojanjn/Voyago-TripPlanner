namespace TripPlanner.Dtos.ItineraryItem;

public class ItineraryItemDto
{
    public int Id { get; set; }
    public DateTime StartDateTime { get; set; }
    public DateTime EndDateTime { get; set; }
    public int StopOrder { get; set; }
    public string? Note { get; set; }

}