using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ATLManager.Migrations
{
    public partial class Refeicao_AtlId_NonNullable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Refeicao_ATL_AtlId",
                table: "Refeicao");

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Refeicao_ATL_AtlId",
                table: "Refeicao");

            migrationBuilder.AlterColumn<Guid>(
                name: "AtlId",
                table: "Refeicao",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_Refeicao_ATL_AtlId",
                table: "Refeicao",
                column: "AtlId",
                principalTable: "ATL",
                principalColumn: "AtlId");
        }
    }
}
