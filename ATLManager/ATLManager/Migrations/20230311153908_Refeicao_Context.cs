using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ATLManager.Migrations
{
    public partial class Refeicao_Context : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Refeicao",
                columns: table => new
                {
                    RefeicaoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Categoria = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Data = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Proteina = table.Column<float>(type: "real", nullable: false),
                    HidratosCarbono = table.Column<float>(type: "real", nullable: false),
                    VR = table.Column<float>(type: "real", nullable: false),
                    Acucar = table.Column<float>(type: "real", nullable: false),
                    Lipidos = table.Column<float>(type: "real", nullable: false),
                    ValorEnergetico = table.Column<float>(type: "real", nullable: false),
                    AGSat = table.Column<float>(type: "real", nullable: false),
                    Sal = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Refeicao", x => x.RefeicaoId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Refeicao");
        }
    }
}
