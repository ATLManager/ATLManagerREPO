using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ATLManager.Migrations
{
    public partial class Refeicao_AtlId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AtlId",
                table: "Refeicao",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Refeicao_AtlId",
                table: "Refeicao",
                column: "AtlId");

            migrationBuilder.AddForeignKey(
                name: "FK_Refeicao_ATL_AtlId",
                table: "Refeicao",
                column: "AtlId",
                principalTable: "ATL",
                principalColumn: "AtlId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Refeicao_ATL_AtlId",
                table: "Refeicao");

            migrationBuilder.DropIndex(
                name: "IX_Refeicao_AtlId",
                table: "Refeicao");

            migrationBuilder.DropColumn(
                name: "AtlId",
                table: "Refeicao");
        }
    }
}
