using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ATLManager.Migrations
{
    public partial class Recibos_ATL_FK : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AtlId",
                table: "Recibo",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Recibo_AtlId",
                table: "Recibo",
                column: "AtlId");

            migrationBuilder.AddForeignKey(
                name: "FK_Recibo_ATL_AtlId",
                table: "Recibo",
                column: "AtlId",
                principalTable: "ATL",
                principalColumn: "AtlId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Recibo_ATL_AtlId",
                table: "Recibo");

            migrationBuilder.DropIndex(
                name: "IX_Recibo_AtlId",
                table: "Recibo");

            migrationBuilder.DropColumn(
                name: "AtlId",
                table: "Recibo");
        }
    }
}
