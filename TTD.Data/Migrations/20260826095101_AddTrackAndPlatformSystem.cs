using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TTD.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTrackAndPlatformSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Routes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Routes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Stations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PlatformCount = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 2),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Latitude = table.Column<double>(type: "REAL", nullable: false),
                    Longitude = table.Column<double>(type: "REAL", nullable: false),
                    MapX = table.Column<int>(type: "INTEGER", nullable: true),
                    MapY = table.Column<int>(type: "INTEGER", nullable: true),
                    IsPassenger = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsCargo = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Trains",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Model = table.Column<string>(type: "TEXT", nullable: false),
                    VMax = table.Column<int>(type: "INTEGER", nullable: false),
                    Power = table.Column<int>(type: "INTEGER", nullable: false),
                    Weight = table.Column<int>(type: "INTEGER", nullable: false),
                    TractiveEffort = table.Column<int>(type: "INTEGER", nullable: true),
                    ModelYear = table.Column<int>(type: "INTEGER", nullable: false),
                    IsElectric = table.Column<bool>(type: "INTEGER", nullable: false),
                    ImagePath = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trains", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RouteStations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RouteId = table.Column<int>(type: "INTEGER", nullable: false),
                    TrackId = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 1),
                    PlatformId = table.Column<int>(type: "INTEGER", nullable: true),
                    StationId = table.Column<int>(type: "INTEGER", nullable: false),
                    StopOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    StopDuration = table.Column<int>(type: "INTEGER", nullable: false),
                    DistanceFromPrevious = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RouteStations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RouteStations_Routes_RouteId",
                        column: x => x.RouteId,
                        principalTable: "Routes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RouteStations_Stations_StationId",
                        column: x => x.StationId,
                        principalTable: "Stations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Schedules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RouteId = table.Column<int>(type: "INTEGER", nullable: false),
                    TrainId = table.Column<int>(type: "INTEGER", nullable: false),
                    DepartureTime = table.Column<TimeSpan>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    ValidFrom = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ValidTo = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Notes = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Schedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Schedules_Routes_RouteId",
                        column: x => x.RouteId,
                        principalTable: "Routes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Schedules_Trains_TrainId",
                        column: x => x.TrainId,
                        principalTable: "Trains",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ScheduleTravelTimes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ScheduleId = table.Column<int>(type: "INTEGER", nullable: false),
                    RouteStationId = table.Column<int>(type: "INTEGER", nullable: false),
                    TravelTimeMinutes = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduleTravelTimes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScheduleTravelTimes_RouteStations_RouteStationId",
                        column: x => x.RouteStationId,
                        principalTable: "RouteStations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ScheduleTravelTimes_Schedules_ScheduleId",
                        column: x => x.ScheduleId,
                        principalTable: "Schedules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RouteStation_StopOrder",
                table: "RouteStations",
                column: "StopOrder");

            migrationBuilder.CreateIndex(
                name: "IX_RouteStation_Unique",
                table: "RouteStations",
                columns: new[] { "RouteId", "StationId", "StopOrder" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RouteStations_StationId",
                table: "RouteStations",
                column: "StationId");

            migrationBuilder.CreateIndex(
                name: "IX_Schedule_Route_Departure",
                table: "Schedules",
                columns: new[] { "RouteId", "DepartureTime" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Schedule_TrainId",
                table: "Schedules",
                column: "TrainId");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleTravelTimes_RouteStationId",
                table: "ScheduleTravelTimes",
                column: "RouteStationId");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleTravelTimes_ScheduleId",
                table: "ScheduleTravelTimes",
                column: "ScheduleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ScheduleTravelTimes");

            migrationBuilder.DropTable(
                name: "RouteStations");

            migrationBuilder.DropTable(
                name: "Schedules");

            migrationBuilder.DropTable(
                name: "Stations");

            migrationBuilder.DropTable(
                name: "Routes");

            migrationBuilder.DropTable(
                name: "Trains");
        }
    }
}
