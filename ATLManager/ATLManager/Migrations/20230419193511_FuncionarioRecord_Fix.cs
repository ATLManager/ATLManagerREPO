using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ATLManager.Migrations
{
    public partial class FuncionarioRecord_Fix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FuncionarioRecord_AspNetUsers_UserId",
                table: "FuncionarioRecord");

            migrationBuilder.DropIndex(
                name: "IX_FuncionarioRecord_UserId",
                table: "FuncionarioRecord");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "FuncionarioRecord");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "FuncionarioRecord",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "FuncionarioRecord",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "FuncionarioRecord",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "FuncionarioRecord");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "FuncionarioRecord");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "FuncionarioRecord");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "FuncionarioRecord",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_FuncionarioRecord_UserId",
                table: "FuncionarioRecord",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_FuncionarioRecord_AspNetUsers_UserId",
                table: "FuncionarioRecord",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
