using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ATLManager.Migrations
{
    public partial class RefeicaoRecord : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RefeicaoRecord",
                columns: table => new
                {
                    RefeicaoRecordId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RefeicaoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Categoria = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Data = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Proteina = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HidratosCarbono = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VR = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Acucar = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Lipidos = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ValorEnergetico = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AGSat = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Sal = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Picture = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AtlId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefeicaoRecord", x => x.RefeicaoRecordId);
                    table.ForeignKey(
                        name: "FK_RefeicaoRecord_ATL_AtlId",
                        column: x => x.AtlId,
                        principalTable: "ATL",
                        principalColumn: "AtlId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_RefeicaoRecord_AtlId",
                table: "RefeicaoRecord",
                column: "AtlId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RefeicaoRecord");
        }
    }
}
