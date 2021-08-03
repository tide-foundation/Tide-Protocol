using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Tide.Simulator.Migrations
{
    public partial class AddAuthentication : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AuthPendings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TranId = table.Column<Guid>(nullable: false),
                    Uid = table.Column<Guid>(nullable: false),
                    OrkId = table.Column<string>(type: "VARCHAR(400)", nullable: true),
                    Method = table.Column<string>(type: "VARCHAR(250)", nullable: true),
                    Successful = table.Column<bool>(nullable: false),
                    Time = table.Column<DateTimeOffset>(nullable: false),
                    Metadata = table.Column<string>(type: "VARCHAR(MAX)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthPendings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Auths",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TranId = table.Column<Guid>(nullable: false),
                    Uid = table.Column<Guid>(nullable: false),
                    SuccessfulOrks = table.Column<string>(type: "VARCHAR(MAX)", nullable: true),
                    UnsuccessfulOrks = table.Column<string>(type: "VARCHAR(MAX)", nullable: true),
                    Method = table.Column<string>(type: "VARCHAR(250)", nullable: true),
                    Successful = table.Column<bool>(nullable: false),
                    Time = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Auths", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuthPendings");

            migrationBuilder.DropTable(
                name: "Auths");
        }
    }
}
