using System.ComponentModel.DataAnnotations;

namespace TripPlanner.Dtos.ItineraryItem;

public class UpdateItineraryItemDto : IValidatableObject
{
    public DateTime StartDateTime { get; set; }
    public DateTime EndDateTime { get; set; }
    public int StopOrder { get; set; }
    public string? Note { get; set; }
    
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (EndDateTime <= StartDateTime)
            yield return new ValidationResult(
                "End time must be after start time",
                new[] { nameof(EndDateTime) }
            );
        
        if (StartDateTime.Year < 2000 || StartDateTime.Year > 3000)
            yield return new ValidationResult(
                "Start date must be between year 2000 and 3000",
                new[] { nameof(StartDateTime) }
            );

        if (EndDateTime.Year < 2000 || EndDateTime.Year > 3000)
            yield return new ValidationResult(
                "End date must be between year 2000 and 3000",
                new[] { nameof(EndDateTime) }
            );
    }
}