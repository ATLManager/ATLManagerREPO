using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ATLManager.Migrations
{
    public partial class ATL : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ATL",
                columns: table => new
                {
                    AtlId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    City = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PostalCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AgrupamentoPaiAgrupamentoID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NIPC = table.Column<string>(type: "nvarchar(9)", maxLength: 9, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ATL", x => x.AtlId);
                    table.ForeignKey(
                        name: "FK_ATL_Agrupamento_AgrupamentoPaiAgrupamentoID",
                        column: x => x.AgrupamentoPaiAgrupamentoID,
                        principalTable: "Agrupamento",
                        principalColumn: "AgrupamentoID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EncarregadoEducacao",
                columns: table => new
                {
                    EncarregadoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Phone = table.Column<int>(type: "int", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PostalCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    City = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    NIF = table.Column<int>(type: "int", maxLength: 9, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EncarregadoEducacao", x => x.EncarregadoId);
                    table.ForeignKey(
                        name: "FK_EncarregadoEducacao_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ATL_AgrupamentoPaiAgrupamentoID",
                table: "ATL",
                column: "AgrupamentoPaiAgrupamentoID");

            migrationBuilder.CreateIndex(
                name: "IX_EncarregadoEducacao_UserId",
                table: "EncarregadoEducacao",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ATL");

            migrationBuilder.DropTable(
                name: "EncarregadoEducacao");
        }
    }
}
