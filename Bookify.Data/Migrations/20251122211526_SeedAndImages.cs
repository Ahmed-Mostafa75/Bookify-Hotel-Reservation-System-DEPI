using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Bookify.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedAndImages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "RoomTypes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.InsertData(
                table: "RoomTypes",
                columns: new[] { "Id", "BasePricePerNight", "Capacity", "Description", "ImageUrl", "Name" },
                values: new object[,]
                {
                    { 1, 80m, 1, "Cozy single room", "https://images.unsplash.com/photo-1505692794403-34d4982c724a?w=1200&q=80", "Single" },
                    { 2, 120m, 2, "Comfortable double room", "https://images.unsplash.com/photo-1505691938895-1758d7feb511?w=1200&q=80", "Double" },
                    { 3, 240m, 4, "Spacious suite with living area", "https://images.unsplash.com/photo-1554995207-80a3a44b9e41?w=1200&q=80", "Suite" }
                });

            migrationBuilder.InsertData(
                table: "Rooms",
                columns: new[] { "Id", "IsAvailable", "Number", "RoomTypeId" },
                values: new object[,]
                {
                    { 1, true, "101", 1 },
                    { 2, true, "102", 1 },
                    { 3, true, "201", 2 },
                    { 4, true, "202", 2 },
                    { 5, true, "301", 3 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "RoomTypes",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "RoomTypes",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "RoomTypes",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "RoomTypes");
        }
    }
}
