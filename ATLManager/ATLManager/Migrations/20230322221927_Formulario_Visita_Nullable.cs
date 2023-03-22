using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ATLManager.Migrations
{
    public partial class Formulario_Visita_Nullable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FormularioID",
                table: "Formulario",
                newName: "FormularioId");

            migrationBuilder.AddColumn<Guid>(
                name: "VisitaEstudoId",
                table: "Formulario",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Formulario_VisitaEstudoId",
                table: "Formulario",
                column: "VisitaEstudoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Formulario_VisitaEstudo_VisitaEstudoId",
                table: "Formulario",
                column: "VisitaEstudoId",
                principalTable: "VisitaEstudo",
                principalColumn: "VisitaEstudoID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Formulario_VisitaEstudo_VisitaEstudoId",
                table: "Formulario");

            migrationBuilder.DropIndex(
                name: "IX_Formulario_VisitaEstudoId",
                table: "Formulario");

            migrationBuilder.DropColumn(
                name: "VisitaEstudoId",
                table: "Formulario");

            migrationBuilder.RenameColumn(
                name: "FormularioId",
                table: "Formulario",
                newName: "FormularioID");
        }
    }
}
