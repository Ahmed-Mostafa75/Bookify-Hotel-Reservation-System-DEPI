using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bookify.Data.Migrations
{
    /// <inheritdoc />
    public partial class updatesingle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "RoomTypes",
                keyColumn: "Id",
                keyValue: 1,
                column: "ImageUrl",
                value: "../images/singles.jpg");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "RoomTypes",
                keyColumn: "Id",
                keyValue: 1,
                column: "ImageUrl",
                value: "https://images.unsplash.com/photo-1505692794403-34d4982c724a?w=1200&q=80");
        }
    }
}
