namespace TripPlanner.Dtos.Route;

public class RouteDto
{
    public string Polyline { get; set; } = "";
    public string TotalDistance { get; set; } = "";
    public string TotalDuration { get; set; } = "";
    public List<LegDto> Legs { get; set; } = new();
}

public class LegDto
{
    public string Distance { get; set; } = "";
    public string Duration { get; set; } = "";
}