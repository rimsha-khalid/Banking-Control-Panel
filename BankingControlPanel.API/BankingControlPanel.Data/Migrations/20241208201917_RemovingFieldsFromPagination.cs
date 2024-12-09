using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BankingControlPanel.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemovingFieldsFromPagination : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FilterFirstName",
                table: "Paginations");

            migrationBuilder.DropColumn(
                name: "FilterLastName",
                table: "Paginations");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FilterFirstName",
                table: "Paginations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FilterLastName",
                table: "Paginations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
