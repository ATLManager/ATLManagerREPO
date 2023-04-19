using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ATLManager.Migrations
{
    public partial class AtividadeRecord : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AtividadeRecord",
                columns: table => new
                {
                    AtividadeRecordId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AtividadeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Picture = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AtlId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AtividadeRecord", x => x.AtividadeRecordId);
                    table.ForeignKey(
                        name: "FK_AtividadeRecord_ATL_AtlId",
                        column: x => x.AtlId,
                        principalTable: "ATL",
                        principalColumn: "AtlId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AtividadeRecord_AtlId",
                table: "AtividadeRecord",
                column: "AtlId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AtividadeRecord");
        }
    }
}
