using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bookify.Data.Migrations
{
    /// <inheritdoc />
    public partial class DarkThemeAmenities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Amenities",
                table: "RoomTypes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "RoomTypes",
                keyColumn: "Id",
                keyValue: 1,
                column: "Amenities",
                value: "Wi-Fi,Air Conditioning,TV");

            migrationBuilder.UpdateData(
                table: "RoomTypes",
                keyColumn: "Id",
                keyValue: 2,
                column: "Amenities",
                value: "Wi-Fi,Air Conditioning,Mini Fridge,TV");

            migrationBuilder.UpdateData(
                table: "RoomTypes",
                keyColumn: "Id",
                keyValue: 3,
                column: "Amenities",
                value: "Wi-Fi,Air Conditioning,Living Area,Mini Bar,Smart TV");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Amenities",
                table: "RoomTypes");
        }
    }
}
