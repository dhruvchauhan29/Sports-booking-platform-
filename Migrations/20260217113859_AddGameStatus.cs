using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SportsBookingPlatform.Migrations
{
    /// <inheritdoc />
    public partial class AddGameStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Games",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Games");
        }
    }
}
