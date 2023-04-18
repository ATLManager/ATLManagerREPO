using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ATLManager.Migrations
{
    public partial class Atividade_ATL_Navigation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Atividade_AtlId",
                table: "Atividade",
                column: "AtlId");

            migrationBuilder.AddForeignKey(
                name: "FK_Atividade_ATL_AtlId",
                table: "Atividade",
                column: "AtlId",
                principalTable: "ATL",
                principalColumn: "AtlId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Atividade_ATL_AtlId",
                table: "Atividade");

            migrationBuilder.DropIndex(
                name: "IX_Atividade_AtlId",
                table: "Atividade");
        }
    }
}
