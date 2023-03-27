using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ATLManager.Migrations
{
    public partial class Agrupamento_ContaId_FK : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ContaId",
                table: "Agrupamento",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Agrupamento_ContaId",
                table: "Agrupamento",
                column: "ContaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Agrupamento_ContaAdministrativa_ContaId",
                table: "Agrupamento",
                column: "ContaId",
                principalTable: "ContaAdministrativa",
                principalColumn: "ContaId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Agrupamento_ContaAdministrativa_ContaId",
                table: "Agrupamento");

            migrationBuilder.DropIndex(
                name: "IX_Agrupamento_ContaId",
                table: "Agrupamento");

            migrationBuilder.DropColumn(
                name: "ContaId",
                table: "Agrupamento");
        }
    }
}
