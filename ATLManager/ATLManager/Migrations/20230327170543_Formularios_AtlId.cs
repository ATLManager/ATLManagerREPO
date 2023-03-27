using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ATLManager.Migrations
{
    public partial class Formularios_AtlId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AtlId",
                table: "Formulario",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Formulario_AtlId",
                table: "Formulario",
                column: "AtlId");

            migrationBuilder.AddForeignKey(
                name: "FK_Formulario_ATL_AtlId",
                table: "Formulario",
                column: "AtlId",
                principalTable: "ATL",
                principalColumn: "AtlId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Formulario_ATL_AtlId",
                table: "Formulario");

            migrationBuilder.DropIndex(
                name: "IX_Formulario_AtlId",
                table: "Formulario");

            migrationBuilder.DropColumn(
                name: "AtlId",
                table: "Formulario");
        }
    }
}
