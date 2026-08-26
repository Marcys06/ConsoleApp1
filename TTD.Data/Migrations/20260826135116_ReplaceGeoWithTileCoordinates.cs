using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TTD.Data.Migrations
{
    /// <inheritdoc />
    public partial class ReplaceGeoWithTileCoordinates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Stations");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Stations");

            migrationBuilder.DropColumn(
                name: "MapX",
                table: "Stations");

            migrationBuilder.DropColumn(
                name: "MapY",
                table: "Stations");

            migrationBuilder.AddColumn<int>(
                name: "TileX",
                table: "Stations",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TileY",
                table: "Stations",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TileX",
                table: "Stations");

            migrationBuilder.DropColumn(
                name: "TileY",
                table: "Stations");

            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "Stations",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "Stations",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "MapX",
                table: "Stations",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MapY",
                table: "Stations",
                type: "INTEGER",
                nullable: true);
        }
    }
}
