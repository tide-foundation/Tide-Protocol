using Microsoft.EntityFrameworkCore.Migrations;

namespace Tide.BlockchainSimulator.Migrations
{
    public partial class Initial4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DataIndex",
                table: "Data");

            migrationBuilder.RenameColumn(
                name: "Scope",
                table: "Data",
                newName: "Index");

            migrationBuilder.AlterColumn<int>(
                name: "Table",
                table: "Data",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Contract",
                table: "Data",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Contract",
                table: "Data");

            migrationBuilder.RenameColumn(
                name: "Index",
                table: "Data",
                newName: "Scope");

            migrationBuilder.AlterColumn<string>(
                name: "Table",
                table: "Data",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<int>(
                name: "DataIndex",
                table: "Data",
                nullable: false,
                defaultValue: 0);
        }
    }
}
