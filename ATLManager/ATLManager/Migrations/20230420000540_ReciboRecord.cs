using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ATLManager.Migrations
{
    public partial class ReciboRecord : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ReciboRecord",
                columns: table => new
                {
                    ReciboRecordId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NIB = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmissionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateLimit = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AtlId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReciboRecord", x => x.ReciboRecordId);
                    table.ForeignKey(
                        name: "FK_ReciboRecord_ATL_AtlId",
                        column: x => x.AtlId,
                        principalTable: "ATL",
                        principalColumn: "AtlId");
                });

            migrationBuilder.CreateTable(
                name: "ReciboRespostaRecord",
                columns: table => new
                {
                    ReciboRespostaRecordId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReciboRecordId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Educando = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NIB = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateLimit = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Authorized = table.Column<bool>(type: "bit", nullable: false),
                    ResponseDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ComprovativoPath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReceiptPath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReciboRespostaRecord", x => x.ReciboRespostaRecordId);
                    table.ForeignKey(
                        name: "FK_ReciboRespostaRecord_ReciboRecord_ReciboRecordId",
                        column: x => x.ReciboRecordId,
                        principalTable: "ReciboRecord",
                        principalColumn: "ReciboRecordId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReciboRecord_AtlId",
                table: "ReciboRecord",
                column: "AtlId");

            migrationBuilder.CreateIndex(
                name: "IX_ReciboRespostaRecord_ReciboRecordId",
                table: "ReciboRespostaRecord",
                column: "ReciboRecordId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReciboRespostaRecord");

            migrationBuilder.DropTable(
                name: "ReciboRecord");
        }
    }
}
