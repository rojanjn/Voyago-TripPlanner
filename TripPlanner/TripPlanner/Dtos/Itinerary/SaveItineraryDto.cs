namespace TripPlanner.Dtos.Itinerary;
using System.ComponentModel.DataAnnotations;


public class SaveItineraryDto
{
    [Required]
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    [Required]
    public DateTime StartDate { get; set; }
    [Required]
    public DateTime EndDate { get; set; }
    public int? CountryId { get; set; }
}