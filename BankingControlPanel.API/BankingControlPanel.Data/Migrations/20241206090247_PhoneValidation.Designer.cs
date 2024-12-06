﻿// <auto-generated />
using System;
using BankingControlPanel.Data.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace BankingControlPanel.Data.Migrations
{
    [DbContext(typeof(BankingControlPanelDbContext))]
    [Migration("20241206090247_PhoneValidation")]
    partial class PhoneValidation
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("BankingControlPanel.Data.Models.Account", b =>
                {
                    b.Property<int>("AccountId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("AccountId"));

                    b.Property<string>("AccountNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("AccountType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("Balance")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("ClientId")
                        .HasColumnType("int");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.HasKey("AccountId");

                    b.HasIndex("ClientId");

                    b.ToTable("Accounts");
                });

            modelBuilder.Entity("BankingControlPanel.Data.Models.Address", b =>
                {
                    b.Property<int>("AddressId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("AddressId"));

                    b.Property<string>("City")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ClientId")
                        .HasColumnType("int");

                    b.Property<string>("Country")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<string>("Street")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ZipCode")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("AddressId");

                    b.HasIndex("ClientId")
                        .IsUnique();

                    b.ToTable("Addresses");
                });

            modelBuilder.Entity("BankingControlPanel.Data.Models.Client", b =>
                {
                    b.Property<int>("ClientId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ClientId"));

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(59)
                        .HasColumnType("nvarchar(59)");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(59)
                        .HasColumnType("nvarchar(59)");

                    b.Property<string>("MobileNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PersonalId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ProfilePhoto")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Sex")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("UserId")
                        .HasColumnType("int");

                    b.HasKey("ClientId");

                    b.HasIndex("UserId")
                        .IsUnique()
                        .HasFilter("[UserId] IS NOT NULL");

                    b.ToTable("Clients");
                });

            modelBuilder.Entity("BankingControlPanel.Data.Models.Filter", b =>
                {
                    b.Property<int>("FilterId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("FilterId"));

                    b.Property<DateTime>("FilterAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("FilterParams")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsSuccessful")
                        .HasColumnType("bit");

                    b.HasKey("FilterId");

                    b.ToTable("Filters");
                });

            modelBuilder.Entity("BankingControlPanel.Data.Models.Search", b =>
                {
                    b.Property<int>("SearchId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("SearchId"));

                    b.Property<bool>("IsSuccessful")
                        .HasColumnType("bit");

                    b.Property<DateTime>("SearchAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("SearchRecord")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("SearchId");

                    b.ToTable("Searches");
                });

            modelBuilder.Entity("BankingControlPanel.Data.Models.User", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("UserId"));

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("BankingControlPanel.Data.Models.Account", b =>
                {
                    b.HasOne("BankingControlPanel.Data.Models.Client", "Client")
                        .WithMany("Account")
                        .HasForeignKey("ClientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Client");
                });

            modelBuilder.Entity("BankingControlPanel.Data.Models.Address", b =>
                {
                    b.HasOne("BankingControlPanel.Data.Models.Client", "Client")
                        .WithOne("Address")
                        .HasForeignKey("BankingControlPanel.Data.Models.Address", "ClientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Client");
                });

            modelBuilder.Entity("BankingControlPanel.Data.Models.Client", b =>
                {
                    b.HasOne("BankingControlPanel.Data.Models.User", "User")
                        .WithOne("Client")
                        .HasForeignKey("BankingControlPanel.Data.Models.Client", "UserId");

                    b.Navigation("User");
                });

            modelBuilder.Entity("BankingControlPanel.Data.Models.Client", b =>
                {
                    b.Navigation("Account");

                    b.Navigation("Address")
                        .IsRequired();
                });

            modelBuilder.Entity("BankingControlPanel.Data.Models.User", b =>
                {
                    b.Navigation("Client");
                });
#pragma warning restore 612, 618
        }
    }
}