using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorldMonitor.Api.Migrations
{
    /// <inheritdoc />
    public partial class RemoveCreatedAt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "FeedSources");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "FeedSources",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "FeedSources",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 15, 4, 59, 35, 707, DateTimeKind.Utc).AddTicks(2810));

            migrationBuilder.UpdateData(
                table: "FeedSources",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 15, 4, 59, 35, 707, DateTimeKind.Utc).AddTicks(3582));

            migrationBuilder.UpdateData(
                table: "FeedSources",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 15, 4, 59, 35, 707, DateTimeKind.Utc).AddTicks(3584));
        }
    }
}
