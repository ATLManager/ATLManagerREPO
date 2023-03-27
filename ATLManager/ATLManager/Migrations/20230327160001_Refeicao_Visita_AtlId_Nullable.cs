using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ATLManager.Migrations
{
    public partial class Refeicao_Visita_AtlId_Nullable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Refeicao_ATL_AtlId",
                table: "Refeicao");

            migrationBuilder.AddColumn<Guid>(
                name: "AtlId",
                table: "VisitaEstudo",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "AtlId",
                table: "Refeicao",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.CreateIndex(
                name: "IX_VisitaEstudo_AtlId",
                table: "VisitaEstudo",
                column: "AtlId");

            migrationBuilder.AddForeignKey(
                name: "FK_Refeicao_ATL_AtlId",
                table: "Refeicao",
                column: "AtlId",
                principalTable: "ATL",
                principalColumn: "AtlId");

            migrationBuilder.AddForeignKey(
                name: "FK_VisitaEstudo_ATL_AtlId",
                table: "VisitaEstudo",
                column: "AtlId",
                principalTable: "ATL",
                principalColumn: "AtlId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Refeicao_ATL_AtlId",
                table: "Refeicao");

            migrationBuilder.DropForeignKey(
                name: "FK_VisitaEstudo_ATL_AtlId",
                table: "VisitaEstudo");

            migrationBuilder.DropIndex(
                name: "IX_VisitaEstudo_AtlId",
                table: "VisitaEstudo");

            migrationBuilder.DropColumn(
                name: "AtlId",
                table: "VisitaEstudo");

            migrationBuilder.AlterColumn<Guid>(
                name: "AtlId",
                table: "Refeicao",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Refeicao_ATL_AtlId",
                table: "Refeicao",
                column: "AtlId",
                principalTable: "ATL",
                principalColumn: "AtlId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
