namespace TripPlanner.Dtos.Itinerary
{
    public class AddAttractionDto
    {
        public int ItineraryId { get; set; }
        public string PlaceId { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Address { get; set; } = null!;
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
    }
}