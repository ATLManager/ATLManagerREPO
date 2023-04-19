using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ATLManager.Migrations
{
    public partial class VisitaEstudoRecord : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "VisitaEstudoRecord",
                columns: table => new
                {
                    VisitaEstudoRecordID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VisitaEstudoID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Picture = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AtlId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VisitaEstudoRecord", x => x.VisitaEstudoRecordID);
                    table.ForeignKey(
                        name: "FK_VisitaEstudoRecord_ATL_AtlId",
                        column: x => x.AtlId,
                        principalTable: "ATL",
                        principalColumn: "AtlId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_VisitaEstudoRecord_AtlId",
                table: "VisitaEstudoRecord",
                column: "AtlId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VisitaEstudoRecord");
        }
    }
}
