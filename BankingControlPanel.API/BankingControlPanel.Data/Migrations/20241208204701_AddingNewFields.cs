using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BankingControlPanel.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddingNewFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TotalPages",
                table: "Paginations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TotalRecords",
                table: "Paginations",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalPages",
                table: "Paginations");

            migrationBuilder.DropColumn(
                name: "TotalRecords",
                table: "Paginations");
        }
    }
}
