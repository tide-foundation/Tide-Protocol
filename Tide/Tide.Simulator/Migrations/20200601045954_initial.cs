using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Tide.Simulator.Migrations {
    public partial class initial : Migration {
        protected override void Up(MigrationBuilder migrationBuilder) {
            migrationBuilder.CreateTable(
                "Data",
                table => new {
                    Id = table.Column<int>()
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateCreated = table.Column<DateTimeOffset>(),
                    Stale = table.Column<bool>(),
                    Contract = table.Column<int>(),
                    Table = table.Column<int>(),
                    Scope = table.Column<string>(nullable: true),
                    Index = table.Column<string>(nullable: true),
                    Data = table.Column<string>(nullable: true)
                },
                constraints: table => { table.PrimaryKey("PK_Data", x => x.Id); });
        }

        protected override void Down(MigrationBuilder migrationBuilder) {
            migrationBuilder.DropTable(
                "Data");
        }
    }
}