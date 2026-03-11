namespace TripPlanner.Google.Directions;

public class DirectionsResponse
{
    public List<Route> Routes { get; set; } = new();
}

public class Route
{
    public OverviewPolyline Overview_Polyline { get; set; } = new();
    public List<Leg> Legs { get; set; } = new();
}

public class OverviewPolyline
{
    public string Points { get; set; } = "";
}

public class Leg
{
    public Distance Distance { get; set; } = new();
    public Duration Duration { get; set; } = new();
}

public class Distance
{
    public string Text { get; set; } = "";
}

public class Duration
{
    public string Text { get; set; } = "";
}