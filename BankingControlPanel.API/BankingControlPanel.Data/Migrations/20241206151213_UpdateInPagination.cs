using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BankingControlPanel.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateInPagination : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Paginations_Users_UserId",
                table: "Paginations");

            migrationBuilder.DropIndex(
                name: "IX_Paginations_UserId",
                table: "Paginations");

            migrationBuilder.DropColumn(
                name: "RequestedAt",
                table: "Paginations");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Paginations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Paginations",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<DateTime>(
                name: "RequestedAt",
                table: "Paginations",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_Paginations_UserId",
                table: "Paginations",
                column: "UserId",
                unique: true,
                filter: "[UserId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Paginations_Users_UserId",
                table: "Paginations",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId");
        }
    }
}
