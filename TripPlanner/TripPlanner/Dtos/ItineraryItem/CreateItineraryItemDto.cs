using System.ComponentModel.DataAnnotations;

namespace TripPlanner.Dtos.ItineraryItem;

public class CreateItineraryItemDto
{
    [Required]
    public int LocationId { get; set; }
    
    [Required]
    public DateTime StartDateTime { get; set; }
    
    
    public DateTime EndDateTime { get; set; }
    
    [Required]
    public int StopOrder { get; set; }
    
    public string? Note { get; set; }
    
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (EndDateTime <= StartDateTime)
            yield return new ValidationResult(
                "End time must be after start time",
                new[] { nameof(EndDateTime) }
            );
    }
}