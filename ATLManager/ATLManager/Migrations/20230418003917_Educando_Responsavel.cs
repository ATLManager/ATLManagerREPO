using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ATLManager.Migrations
{
    public partial class Educando_Responsavel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EducandoResponsavel",
                columns: table => new
                {
                    EducandoResponsavelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EducandoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    Apelido = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    CC = table.Column<string>(type: "nvarchar(9)", maxLength: 9, nullable: false),
                    Phone = table.Column<int>(type: "int", nullable: false),
                    ProfilePicture = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Parentesco = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EducandoResponsavel", x => x.EducandoResponsavelId);
                    table.ForeignKey(
                        name: "FK_EducandoResponsavel_Educando_EducandoId",
                        column: x => x.EducandoId,
                        principalTable: "Educando",
                        principalColumn: "EducandoId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EducandoResponsavel_EducandoId",
                table: "EducandoResponsavel",
                column: "EducandoId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EducandoResponsavel");
        }
    }
}
