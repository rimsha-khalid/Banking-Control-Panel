using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BankingControlPanel.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemovingUserIdFromPagination : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Paginations");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Paginations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
