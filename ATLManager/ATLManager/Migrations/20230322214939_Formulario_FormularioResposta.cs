using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ATLManager.Migrations
{
    public partial class Formulario_FormularioResposta : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Formulario",
                columns: table => new
                {
                    FormularioID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateLimit = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Formulario", x => x.FormularioID);
                });

            migrationBuilder.CreateTable(
                name: "FormularioResposta",
                columns: table => new
                {
                    FormularioRespostaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FormularioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EducandoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Authorized = table.Column<bool>(type: "bit", nullable: false),
                    ResponseDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormularioResposta", x => x.FormularioRespostaId);
                    table.ForeignKey(
                        name: "FK_FormularioResposta_Educando_EducandoId",
                        column: x => x.EducandoId,
                        principalTable: "Educando",
                        principalColumn: "EducandoId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FormularioResposta_Formulario_FormularioId",
                        column: x => x.FormularioId,
                        principalTable: "Formulario",
                        principalColumn: "FormularioID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FormularioResposta_EducandoId",
                table: "FormularioResposta",
                column: "EducandoId");

            migrationBuilder.CreateIndex(
                name: "IX_FormularioResposta_FormularioId",
                table: "FormularioResposta",
                column: "FormularioId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FormularioResposta");

            migrationBuilder.DropTable(
                name: "Formulario");
        }
    }
}
