using Microsoft.EntityFrameworkCore.Migrations;

namespace Tide.Vendor.Migrations
{
    public partial class initial3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Applications_Users_ApplicationUserId",
                table: "Applications");

            migrationBuilder.DropIndex(
                name: "IX_Applications_ApplicationUserId",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Applications");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "Applications",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Applications_ApplicationUserId",
                table: "Applications",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Applications_Users_ApplicationUserId",
                table: "Applications",
                column: "ApplicationUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
