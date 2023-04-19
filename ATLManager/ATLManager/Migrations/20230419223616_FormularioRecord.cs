using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ATLManager.Migrations
{
    public partial class FormularioRecord : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FormularioRecord",
                columns: table => new
                {
                    FormularioRecordId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FormularioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VisitaEstudo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Atividade = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateLimit = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AtlId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormularioRecord", x => x.FormularioRecordId);
                    table.ForeignKey(
                        name: "FK_FormularioRecord_ATL_AtlId",
                        column: x => x.AtlId,
                        principalTable: "ATL",
                        principalColumn: "AtlId");
                });

            migrationBuilder.CreateTable(
                name: "FormularioRespostaRecord",
                columns: table => new
                {
                    FormularioRespostaRecordId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FormularioRespostaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FormularioRecordId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Educando = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Authorized = table.Column<bool>(type: "bit", nullable: false),
                    DateLimit = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ResponseDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormularioRespostaRecord", x => x.FormularioRespostaRecordId);
                    table.ForeignKey(
                        name: "FK_FormularioRespostaRecord_FormularioRecord_FormularioRecordId",
                        column: x => x.FormularioRecordId,
                        principalTable: "FormularioRecord",
                        principalColumn: "FormularioRecordId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FormularioRecord_AtlId",
                table: "FormularioRecord",
                column: "AtlId");

            migrationBuilder.CreateIndex(
                name: "IX_FormularioRespostaRecord_FormularioRecordId",
                table: "FormularioRespostaRecord",
                column: "FormularioRecordId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FormularioRespostaRecord");

            migrationBuilder.DropTable(
                name: "FormularioRecord");
        }
    }
}
