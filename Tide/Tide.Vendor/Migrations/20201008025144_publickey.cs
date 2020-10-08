using Microsoft.EntityFrameworkCore.Migrations;

namespace Tide.Vendor.Migrations
{
    public partial class publickey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CvkPub",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Field1",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Field2",
                table: "Users");

            migrationBuilder.AddColumn<string>(
                name: "PublicKey",
                table: "Users",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PublicKey",
                table: "Users");

            migrationBuilder.AddColumn<string>(
                name: "CvkPub",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Field1",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Field2",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
