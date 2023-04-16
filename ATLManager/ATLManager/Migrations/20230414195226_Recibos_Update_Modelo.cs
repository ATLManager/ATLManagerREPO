using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ATLManager.Migrations
{
    public partial class Recibos_Update_Modelo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ComprovativoPath",
                table: "Recibo");

            migrationBuilder.DropColumn(
                name: "Confirmed",
                table: "Recibo");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "ReciboResposta",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NIB",
                table: "ReciboResposta",
                type: "nvarchar(21)",
                maxLength: 21,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "ReciboResposta",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "ReciboResposta",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "Recibo",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "ReciboResposta");

            migrationBuilder.DropColumn(
                name: "NIB",
                table: "ReciboResposta");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "ReciboResposta");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "ReciboResposta");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "Recibo");

            migrationBuilder.AddColumn<string>(
                name: "ComprovativoPath",
                table: "Recibo",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Confirmed",
                table: "Recibo",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
