using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ATLManager.Migrations
{
    public partial class ContaAdmin_ATL_FK : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AtlId",
                table: "ContaAdministrativa",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ContaAdministrativa_AtlId",
                table: "ContaAdministrativa",
                column: "AtlId");

            migrationBuilder.AddForeignKey(
                name: "FK_ContaAdministrativa_ATL_AtlId",
                table: "ContaAdministrativa",
                column: "AtlId",
                principalTable: "ATL",
                principalColumn: "AtlId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContaAdministrativa_ATL_AtlId",
                table: "ContaAdministrativa");

            migrationBuilder.DropIndex(
                name: "IX_ContaAdministrativa_AtlId",
                table: "ContaAdministrativa");

            migrationBuilder.DropColumn(
                name: "AtlId",
                table: "ContaAdministrativa");
        }
    }
}
