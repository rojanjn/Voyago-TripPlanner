using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TripPlanner.Migrations
{
    /// <inheritdoc />
    public partial class SeedDataSupplement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "locations",
                columns: new[] { "Id", "Address", "Description", "Latitude", "Longitude", "Name", "PlaceId" },
                values: new object[,]
                {
                    { 5, "1 Austin Terrace, Toronto, ON M5R 1X8", null, 43.6781015m, -79.4094159m, "Casa Loma", null },
                    { 6, "1 Blue Jays Way, Toronto, ON M5V 1J4", null, 43.6416598m, -79.3891976m, "Rogers Centre", null },
                    { 7, "160 Kendal Ave, Toronto, ON M5R 1M3", null, 43.675865m, -79.4109118m, "George Brown Polytechnic - Casa Loma Campus", null },
                    { 8, "200 King St E, Toronto, ON M5A 3W8", null, 43.6514195m, -79.3708678m, "George Brown Polytechnic - St. James Campus", null },
                    { 9, "51 Dockside Dr, Toronto, ON M5A 0B6", null, 43.6441155m, -79.3656226m, "George Brown Polytechnic- Waterfront Campus", null },
                    { 10, "100 Queens Park, Toronto, ON M5S 2C6", null, 43.6657043m, -79.3939679m, "Royal Ontario Museum", null },
                    { 11, "317 Dundas St W, Toronto, ON M5T 1G4", null, 43.6536066m, -79.3925123m, "Art Gallery of Ontario", null },
                    { 12, "288 Bremner Blvd, Toronto, ON M5V 3L9", null, 43.6424036m, -79.3868690m, "Ripley's Aquarium of Canada", null },
                    { 13, "6650 Niagara Pkwy, Niagara Falls, ON L2G 0L0", null, 43.0799120m, -79.0746950m, "Niagara Falls (Horseshoe Falls)", null },
                    { 14, "111 Wellington St, Ottawa, ON K1A 0A9", null, 45.4235937m, -75.7009290m, "Parliament Hill", null },
                    { 15, "Old Quebec, Quebec City, QC G1R", null, 46.8138783m, -71.2079809m, "Old Quebec (Vieux-Québec)", null },
                    { 16, "Vancouver, BC V6G 1Z4", null, 49.3043010m, -123.1443000m, "Stanley Park", null },
                    { 17, "Banff, AB T1L 1K2", null, 51.4968460m, -115.9280560m, "Banff National Park", null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "locations",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "locations",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "locations",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "locations",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "locations",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "locations",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "locations",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "locations",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "locations",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "locations",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "locations",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "locations",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "locations",
                keyColumn: "Id",
                keyValue: 17);
        }
    }
}
