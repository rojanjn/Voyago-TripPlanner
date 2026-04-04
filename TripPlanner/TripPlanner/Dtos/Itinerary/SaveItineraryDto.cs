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
    
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (EndDate <= StartDate)
            yield return new ValidationResult(
                "End date must be after start date.",
                new[] { nameof(EndDate) });

        if (StartDate.Year < 2000 || StartDate.Year > 3000)
            yield return new ValidationResult(
                "Start date must be between year 2000 and 3000.",
                new[] { nameof(StartDate) });

        if (EndDate.Year < 2000 || EndDate.Year > 3000)
            yield return new ValidationResult(
                "End date must be between year 2000 and 3000.",
                new[] { nameof(EndDate) });
    }
}