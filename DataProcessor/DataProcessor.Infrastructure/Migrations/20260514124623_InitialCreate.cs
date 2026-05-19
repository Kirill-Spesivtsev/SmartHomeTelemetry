using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DataProcessor.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "locations",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_locations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "air_quality_metrics",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Co2 = table.Column<int>(type: "integer", nullable: false),
                    Pm25 = table.Column<int>(type: "integer", nullable: false),
                    Humidity = table.Column<int>(type: "integer", nullable: false),
                    LocationId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_air_quality_metrics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_air_quality_metrics_locations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "energy_metrics",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Energy = table.Column<float>(type: "real", nullable: false),
                    LocationId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_energy_metrics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_energy_metrics_locations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "motion_metrics",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MotionDetected = table.Column<bool>(type: "boolean", nullable: false),
                    LocationId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_motion_metrics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_motion_metrics_locations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "locations",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1L, "Bedroom" },
                    { 2L, "Corridor" },
                    { 3L, "Living Room" },
                    { 4L, "Kitchen" },
                    { 5L, "Office" },
                    { 6L, "Garage" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_air_quality_metrics_LocationId_CreatedAt",
                table: "air_quality_metrics",
                columns: new[] { "LocationId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_energy_metrics_LocationId_CreatedAt",
                table: "energy_metrics",
                columns: new[] { "LocationId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_locations_Name",
                table: "locations",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_motion_metrics_LocationId_CreatedAt",
                table: "motion_metrics",
                columns: new[] { "LocationId", "CreatedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "air_quality_metrics");

            migrationBuilder.DropTable(
                name: "energy_metrics");

            migrationBuilder.DropTable(
                name: "motion_metrics");

            migrationBuilder.DropTable(
                name: "locations");
        }
    }
}
