using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Opus127.Dengue.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialDengue : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DengueWeeklyAlerts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EpidemiologicalYear = table.Column<int>(type: "int", nullable: false),
                    EpidemiologicalWeek = table.Column<int>(type: "int", nullable: false),
                    EstimatedCases = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    NotifiedCases = table.Column<int>(type: "int", nullable: false),
                    AlertLevel = table.Column<int>(type: "int", nullable: false),
                    Geocode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SyncedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DengueWeeklyAlerts", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DengueWeeklyAlerts_EpidemiologicalYear_EpidemiologicalWeek",
                table: "DengueWeeklyAlerts",
                columns: new[] { "EpidemiologicalYear", "EpidemiologicalWeek" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DengueWeeklyAlerts");
        }
    }
}
