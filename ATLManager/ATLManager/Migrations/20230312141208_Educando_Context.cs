using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ATLManager.Migrations
{
    public partial class Educando_Context : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BoletimVacinasPath",
                table: "Educando");

            migrationBuilder.DropColumn(
                name: "DeclaracaoMedicaPath",
                table: "Educando");

            migrationBuilder.DropColumn(
                name: "ImagemDados",
                table: "Educando");

            migrationBuilder.DropColumn(
                name: "ImagemNome",
                table: "Educando");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BoletimVacinasPath",
                table: "Educando",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DeclaracaoMedicaPath",
                table: "Educando",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<byte[]>(
                name: "ImagemDados",
                table: "Educando",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<string>(
                name: "ImagemNome",
                table: "Educando",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
