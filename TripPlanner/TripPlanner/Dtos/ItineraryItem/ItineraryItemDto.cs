namespace TripPlanner.Dtos.ItineraryItem;

public class ItineraryItemDto
{
    public int Id { get; set; }
    public int LocationId { get; set; }
    public string LocationName { get; set; } = null!;
    public DateTime StartDateTime { get; set; }
    public DateTime EndDateTime { get; set; }
    public int StopOrder { get; set; }
    public string? Note { get; set; }

}