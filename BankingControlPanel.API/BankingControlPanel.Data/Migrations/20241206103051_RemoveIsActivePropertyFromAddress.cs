﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BankingControlPanel.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveIsActivePropertyFromAddress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Addresses");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Addresses",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
