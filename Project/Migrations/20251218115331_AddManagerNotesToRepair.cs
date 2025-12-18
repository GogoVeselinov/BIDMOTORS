using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project.Migrations
{
    /// <inheritdoc />
    public partial class AddManagerNotesToRepair : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ManagerNotes",
                table: "Repairs",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "PriceSettings",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "CreatedOn",
                value: new DateTime(2025, 12, 18, 11, 53, 30, 351, DateTimeKind.Utc).AddTicks(8476));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ManagerNotes",
                table: "Repairs");

            migrationBuilder.UpdateData(
                table: "PriceSettings",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "CreatedOn",
                value: new DateTime(2025, 12, 17, 7, 25, 29, 628, DateTimeKind.Utc).AddTicks(1822));
        }
    }
}
