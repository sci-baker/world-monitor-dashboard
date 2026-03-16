using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorldMonitor.Api.Migrations
{
    /// <inheritdoc />
    public partial class FixPendingChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "FeedSources",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 11, 3, 57, 2, 468, DateTimeKind.Utc).AddTicks(7195));

            migrationBuilder.UpdateData(
                table: "FeedSources",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 11, 3, 57, 2, 468, DateTimeKind.Utc).AddTicks(8106));

            migrationBuilder.UpdateData(
                table: "FeedSources",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 11, 3, 57, 2, 468, DateTimeKind.Utc).AddTicks(8108));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "FeedSources",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 10, 0, 16, 25, 931, DateTimeKind.Utc).AddTicks(5582));

            migrationBuilder.UpdateData(
                table: "FeedSources",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 10, 0, 16, 25, 931, DateTimeKind.Utc).AddTicks(6415));

            migrationBuilder.UpdateData(
                table: "FeedSources",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 10, 0, 16, 25, 931, DateTimeKind.Utc).AddTicks(6417));
        }
    }
}
