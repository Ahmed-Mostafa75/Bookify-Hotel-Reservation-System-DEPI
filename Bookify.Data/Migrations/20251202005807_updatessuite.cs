using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bookify.Data.Migrations
{
    /// <inheritdoc />
    public partial class updatessuite : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "RoomTypes",
                keyColumn: "Id",
                keyValue: 3,
                column: "ImageUrl",
                value: "../images/Suite.jpg");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "RoomTypes",
                keyColumn: "Id",
                keyValue: 3,
                column: "ImageUrl",
                value: "https://images.unsplash.com/photo-1554995207-80a3a44b9e41?w=1200&q=80");
        }
    }
}
