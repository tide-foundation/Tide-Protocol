﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Tide.Vendor.Migrations
{
    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Applications",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(nullable: true),
                    DateSubmitted = table.Column<DateTimeOffset>(nullable: false),
                    FirstName = table.Column<string>(nullable: true),
                    MiddleName = table.Column<string>(nullable: true),
                    LastName = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    DateOfBirth = table.Column<string>(nullable: true),
                    CurrentAddress = table.Column<string>(nullable: true),
                    CurrentSuburb = table.Column<string>(nullable: true),
                    CurrentState = table.Column<string>(nullable: true),
                    CurrentPostcode = table.Column<string>(nullable: true),
                    PreviousAddress = table.Column<string>(nullable: true),
                    PreviousSuburb = table.Column<string>(nullable: true),
                    PreviousState = table.Column<string>(nullable: true),
                    PreviousPostcode = table.Column<string>(nullable: true),
                    CurrentEmployer = table.Column<string>(nullable: true),
                    CurrentEmployerPhone = table.Column<string>(nullable: true),
                    CurrentEmployerEmail = table.Column<string>(nullable: true),
                    CurrentMonthlyPay = table.Column<string>(nullable: true),
                    PreviousEmployer = table.Column<string>(nullable: true),
                    PreviousEmployerPhone = table.Column<string>(nullable: true),
                    PreviousEmployerEmail = table.Column<string>(nullable: true),
                    PreviousMonthlyPay = table.Column<string>(nullable: true),
                    CreditCardOutstanding = table.Column<string>(nullable: true),
                    PersonalLoanOutstanding = table.Column<string>(nullable: true),
                    OtherLoanOutstanding = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Applications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Vuid = table.Column<string>(nullable: true),
                    PublicKey = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Applications");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
