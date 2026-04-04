using System.ComponentModel.DataAnnotations;
using TripPlanner.Dtos.Location;

namespace TripPlanner.Dtos.ItineraryItem;

public class CreateItineraryItemDto
{
    [Required]
    public CreateLocationDto Location { get; set; } = null!;
    public string? Note { get; set; }
}