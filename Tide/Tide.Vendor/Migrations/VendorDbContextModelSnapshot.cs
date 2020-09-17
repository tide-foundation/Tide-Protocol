﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Tide.Vendor.Models;

namespace Tide.Vendor.Migrations
{
    [DbContext(typeof(VendorDbContext))]
    partial class VendorDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Tide.Vendor.Models.RentalApplication", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("CreditCardOutstanding")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CurrentAddress")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CurrentEmployer")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CurrentEmployerEmail")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CurrentEmployerPhone")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CurrentMonthlyPay")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CurrentPostcode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CurrentState")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CurrentSuburb")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DateOfBirth")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTimeOffset>("DateSubmitted")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FirstName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LastName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("MiddleName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("OtherLoanOutstanding")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PersonalLoanOutstanding")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PreviousAddress")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PreviousEmployer")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PreviousEmployerEmail")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PreviousEmployerPhone")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PreviousMonthlyPay")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PreviousPostcode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PreviousState")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PreviousSuburb")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Applications");
                });

            modelBuilder.Entity("Tide.Vendor.Models.User", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("CvkPub")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Field1")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Field2")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Tide.Vendor.Models.RentalApplication", b =>
                {
                    b.HasOne("Tide.Vendor.Models.User", null)
                        .WithMany("RentalApplications")
                        .HasForeignKey("UserId");
                });
#pragma warning restore 612, 618
        }
    }
}
