using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Tide.BlockchainSimulator.Migrations
{
    public partial class Initial3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "DateCreated",
                table: "Data",
                nullable: false,
                oldClrType: typeof(DateTime));

            migrationBuilder.AddColumn<int>(
                name: "DataIndex",
                table: "Data",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Scope",
                table: "Data",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Stale",
                table: "Data",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Table",
                table: "Data",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DataIndex",
                table: "Data");

            migrationBuilder.DropColumn(
                name: "Scope",
                table: "Data");

            migrationBuilder.DropColumn(
                name: "Stale",
                table: "Data");

            migrationBuilder.DropColumn(
                name: "Table",
                table: "Data");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateCreated",
                table: "Data",
                nullable: false,
                oldClrType: typeof(DateTimeOffset));
        }
    }
}
