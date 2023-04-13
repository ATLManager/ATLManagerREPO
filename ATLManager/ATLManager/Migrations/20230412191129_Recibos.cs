using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ATLManager.Migrations
{
    public partial class Recibos : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Recibo",
                columns: table => new
                {
                    ReciboId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NIB = table.Column<string>(type: "nvarchar(21)", maxLength: 21, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    EmissionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateLimit = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ComprovativoPath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Confirmed = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recibo", x => x.ReciboId);
                });

            migrationBuilder.CreateTable(
                name: "ReciboResposta",
                columns: table => new
                {
                    ReciboRespostaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReciboId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EducandoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Authorized = table.Column<bool>(type: "bit", nullable: false),
                    DateLimit = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ResponseDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ComprovativoPath = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReciboResposta", x => x.ReciboRespostaId);
                    table.ForeignKey(
                        name: "FK_ReciboResposta_Educando_EducandoId",
                        column: x => x.EducandoId,
                        principalTable: "Educando",
                        principalColumn: "EducandoId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReciboResposta_Recibo_ReciboId",
                        column: x => x.ReciboId,
                        principalTable: "Recibo",
                        principalColumn: "ReciboId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReciboResposta_EducandoId",
                table: "ReciboResposta",
                column: "EducandoId");

            migrationBuilder.CreateIndex(
                name: "IX_ReciboResposta_ReciboId",
                table: "ReciboResposta",
                column: "ReciboId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReciboResposta");

            migrationBuilder.DropTable(
                name: "Recibo");
        }
    }
}
