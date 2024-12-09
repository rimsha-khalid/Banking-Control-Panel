using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BankingControlPanel.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddingFieldsToPagination : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PageNumber",
                table: "Paginations");

            migrationBuilder.DropColumn(
                name: "PageSize",
                table: "Paginations");

            migrationBuilder.RenameColumn(
                name: "FilterSex",
                table: "Paginations",
                newName: "Filter");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Filter",
                table: "Paginations",
                newName: "FilterSex");

            migrationBuilder.AddColumn<int>(
                name: "PageNumber",
                table: "Paginations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PageSize",
                table: "Paginations",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
