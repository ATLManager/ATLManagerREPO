using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ATLManager.Migrations
{
    public partial class FuncionarioRecord : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FuncionarioRecord",
                columns: table => new
                {
                    FuncionarioRecordId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CC = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProfilePicture = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AtlId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FuncionarioRecord", x => x.FuncionarioRecordId);
                    table.ForeignKey(
                        name: "FK_FuncionarioRecord_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FuncionarioRecord_ATL_AtlId",
                        column: x => x.AtlId,
                        principalTable: "ATL",
                        principalColumn: "AtlId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_FuncionarioRecord_AtlId",
                table: "FuncionarioRecord",
                column: "AtlId");

            migrationBuilder.CreateIndex(
                name: "IX_FuncionarioRecord_UserId",
                table: "FuncionarioRecord",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FuncionarioRecord");
        }
    }
}
