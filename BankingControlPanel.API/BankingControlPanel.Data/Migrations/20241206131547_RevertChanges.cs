using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BankingControlPanel.Data.Migrations
{
    /// <inheritdoc />
    public partial class RevertChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Clients_User_UserId",
                table: "Clients");

            migrationBuilder.DropForeignKey(
                name: "FK_Pagination_User_UserId",
                table: "Pagination");

            migrationBuilder.DropPrimaryKey(
                name: "PK_User",
                table: "User");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Pagination",
                table: "Pagination");

            migrationBuilder.RenameTable(
                name: "User",
                newName: "Users");

            migrationBuilder.RenameTable(
                name: "Pagination",
                newName: "Paginations");

            migrationBuilder.RenameIndex(
                name: "IX_Pagination_UserId",
                table: "Paginations",
                newName: "IX_Paginations_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Users",
                table: "Users",
                column: "UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Paginations",
                table: "Paginations",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Clients_Users_UserId",
                table: "Clients",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Paginations_Users_UserId",
                table: "Paginations",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Clients_Users_UserId",
                table: "Clients");

            migrationBuilder.DropForeignKey(
                name: "FK_Paginations_Users_UserId",
                table: "Paginations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Users",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Paginations",
                table: "Paginations");

            migrationBuilder.RenameTable(
                name: "Users",
                newName: "User");

            migrationBuilder.RenameTable(
                name: "Paginations",
                newName: "Pagination");

            migrationBuilder.RenameIndex(
                name: "IX_Paginations_UserId",
                table: "Pagination",
                newName: "IX_Pagination_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_User",
                table: "User",
                column: "UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Pagination",
                table: "Pagination",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Clients_User_UserId",
                table: "Clients",
                column: "UserId",
                principalTable: "User",
                principalColumn: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Pagination_User_UserId",
                table: "Pagination",
                column: "UserId",
                principalTable: "User",
                principalColumn: "UserId");
        }
    }
}
