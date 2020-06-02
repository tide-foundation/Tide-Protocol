using Microsoft.EntityFrameworkCore.Migrations;

namespace Tide.Simulator.Migrations {
    public partial class accountadded : Migration {
        protected override void Up(MigrationBuilder migrationBuilder) {
            migrationBuilder.CreateTable(
                "Account",
                table => new {
                    Id = table.Column<int>()
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(nullable: true),
                    Token = table.Column<string>(nullable: true),
                    Hash = table.Column<string>(nullable: true),
                    Salt = table.Column<string>(nullable: true)
                },
                constraints: table => { table.PrimaryKey("PK_Account", x => x.Id); });
        }

        protected override void Down(MigrationBuilder migrationBuilder) {
            migrationBuilder.DropTable(
                "Account");
        }
    }
}