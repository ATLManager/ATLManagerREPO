using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ATLManager.Migrations
{
    public partial class Initialize : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EncarregadoEducacao",
                columns: table => new
                {
                    EncarregadoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Phone = table.Column<int>(type: "int", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PostalCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    City = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    NIF = table.Column<int>(type: "int", maxLength: 9, nullable: false),
                    ProfilePicture = table.Column<string>(type: "nvarchar(max)", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "Notificacoes",
                columns: table => new
                {
                    NotificacaoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Titulo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Mensagem = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Lida = table.Column<bool>(type: "bit", nullable: false),
                    DataNotificacao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notificacoes", x => x.NotificacaoId);
                    table.ForeignKey(
                        name: "FK_Notificacoes_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Agrupamento",
                columns: table => new
                {
                    AgrupamentoID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Location = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    NIPC = table.Column<string>(type: "nvarchar(9)", maxLength: 9, nullable: false),
                    LogoPicture = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContaId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Agrupamento", x => x.AgrupamentoID);
                });

            migrationBuilder.CreateTable(
                name: "ATL",
                columns: table => new
                {
                    AtlId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    City = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PostalCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AgrupamentoId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    NIPC = table.Column<string>(type: "nvarchar(9)", maxLength: 9, nullable: true),
                    LogoPicture = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ATL", x => x.AtlId);
                    table.ForeignKey(
                        name: "FK_ATL_Agrupamento_AgrupamentoId",
                        column: x => x.AgrupamentoId,
                        principalTable: "Agrupamento",
                        principalColumn: "AgrupamentoID");
                });

            migrationBuilder.CreateTable(
                name: "Atividade",
                columns: table => new
                {
                    AtividadeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Picture = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AtlId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Atividade", x => x.AtividadeId);
                    table.ForeignKey(
                        name: "FK_Atividade_ATL_AtlId",
                        column: x => x.AtlId,
                        principalTable: "ATL",
                        principalColumn: "AtlId");
                });

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

            migrationBuilder.CreateTable(
                name: "ContaAdministrativa",
                columns: table => new
                {
                    ContaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CC = table.Column<string>(type: "nvarchar(9)", maxLength: 9, nullable: false),
                    AtlId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ProfilePicture = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContaAdministrativa", x => x.ContaId);
                    table.ForeignKey(
                        name: "FK_ContaAdministrativa_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ContaAdministrativa_ATL_AtlId",
                        column: x => x.AtlId,
                        principalTable: "ATL",
                        principalColumn: "AtlId");
                });

            migrationBuilder.CreateTable(
                name: "Educando",
                columns: table => new
                {
                    EducandoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    Apelido = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    CC = table.Column<string>(type: "nvarchar(9)", maxLength: 9, nullable: false),
                    Genero = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AtlId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EncarregadoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProfilePicture = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DeclaracaoMedica = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BoletimVacinas = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Educando", x => x.EducandoId);
                    table.ForeignKey(
                        name: "FK_Educando_ATL_AtlId",
                        column: x => x.AtlId,
                        principalTable: "ATL",
                        principalColumn: "AtlId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Educando_EncarregadoEducacao_EncarregadoId",
                        column: x => x.EncarregadoId,
                        principalTable: "EncarregadoEducacao",
                        principalColumn: "EncarregadoId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EducandoRecord",
                columns: table => new
                {
                    EducandoRecordId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EducandoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Apelido = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CC = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Genero = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Encarregado = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProfilePicture = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AtlId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EducandoRecord", x => x.EducandoRecordId);
                    table.ForeignKey(
                        name: "FK_EducandoRecord_ATL_AtlId",
                        column: x => x.AtlId,
                        principalTable: "ATL",
                        principalColumn: "AtlId",
                        onDelete: ReferentialAction.Cascade);
                });

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
                name: "FuncionarioRecord",
                columns: table => new
                {
                    FuncionarioRecordId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CC = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProfilePicture = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AtlId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FuncionarioRecord", x => x.FuncionarioRecordId);
                    table.ForeignKey(
                        name: "FK_FuncionarioRecord_ATL_AtlId",
                        column: x => x.AtlId,
                        principalTable: "ATL",
                        principalColumn: "AtlId");
                });

            migrationBuilder.CreateTable(
                name: "Recibo",
                columns: table => new
                {
                    ReciboId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Price = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NIB = table.Column<string>(type: "nvarchar(21)", maxLength: 21, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    EmissionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateLimit = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AtlId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recibo", x => x.ReciboId);
                    table.ForeignKey(
                        name: "FK_Recibo_ATL_AtlId",
                        column: x => x.AtlId,
                        principalTable: "ATL",
                        principalColumn: "AtlId");
                });

            migrationBuilder.CreateTable(
                name: "Refeicao",
                columns: table => new
                {
                    RefeicaoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Categoria = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Data = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
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
                    table.PrimaryKey("PK_Refeicao", x => x.RefeicaoId);
                    table.ForeignKey(
                        name: "FK_Refeicao_ATL_AtlId",
                        column: x => x.AtlId,
                        principalTable: "ATL",
                        principalColumn: "AtlId");
                });

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

            migrationBuilder.CreateTable(
                name: "VisitaEstudo",
                columns: table => new
                {
                    VisitaEstudoID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Picture = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AtlId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VisitaEstudo", x => x.VisitaEstudoID);
                    table.ForeignKey(
                        name: "FK_VisitaEstudo_ATL_AtlId",
                        column: x => x.AtlId,
                        principalTable: "ATL",
                        principalColumn: "AtlId");
                });

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

            migrationBuilder.CreateTable(
                name: "ATLAdmin",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AtlId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ATLAdmin", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ATLAdmin_ATL_AtlId",
                        column: x => x.AtlId,
                        principalTable: "ATL",
                        principalColumn: "AtlId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ATLAdmin_ContaAdministrativa_ContaId",
                        column: x => x.ContaId,
                        principalTable: "ContaAdministrativa",
                        principalColumn: "ContaId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CoordATL",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AtlId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoordATL", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CoordATL_ATL_AtlId",
                        column: x => x.AtlId,
                        principalTable: "ATL",
                        principalColumn: "AtlId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CoordATL_ContaAdministrativa_ContaId",
                        column: x => x.ContaId,
                        principalTable: "ContaAdministrativa",
                        principalColumn: "ContaId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EducandoResponsavel",
                columns: table => new
                {
                    EducandoResponsavelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EducandoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    Apelido = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    CC = table.Column<string>(type: "nvarchar(9)", maxLength: 9, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(9)", maxLength: 9, nullable: false),
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

            migrationBuilder.CreateTable(
                name: "EducandoSaude",
                columns: table => new
                {
                    EducandoSaudeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EducandoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BloodType = table.Column<string>(type: "nvarchar(3)", nullable: true),
                    EmergencyContact = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InsuranceName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InsuranceNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Allergies = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Diseases = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Medication = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MedicalHistory = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EducandoSaude", x => x.EducandoSaudeId);
                    table.ForeignKey(
                        name: "FK_EducandoSaude_Educando_EducandoId",
                        column: x => x.EducandoId,
                        principalTable: "Educando",
                        principalColumn: "EducandoId",
                        onDelete: ReferentialAction.Cascade);
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

            migrationBuilder.CreateTable(
                name: "ReciboResposta",
                columns: table => new
                {
                    ReciboRespostaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReciboId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EducandoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Price = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NIB = table.Column<string>(type: "nvarchar(21)", maxLength: 21, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    DateLimit = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Authorized = table.Column<bool>(type: "bit", nullable: false),
                    ResponseDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ComprovativoPath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReceiptPath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
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

            migrationBuilder.CreateTable(
                name: "Formulario",
                columns: table => new
                {
                    FormularioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    VisitaEstudoId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AtividadeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateLimit = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AtlId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Formulario", x => x.FormularioId);
                    table.ForeignKey(
                        name: "FK_Formulario_Atividade_AtividadeId",
                        column: x => x.AtividadeId,
                        principalTable: "Atividade",
                        principalColumn: "AtividadeId");
                    table.ForeignKey(
                        name: "FK_Formulario_ATL_AtlId",
                        column: x => x.AtlId,
                        principalTable: "ATL",
                        principalColumn: "AtlId");
                    table.ForeignKey(
                        name: "FK_Formulario_VisitaEstudo_VisitaEstudoId",
                        column: x => x.VisitaEstudoId,
                        principalTable: "VisitaEstudo",
                        principalColumn: "VisitaEstudoID");
                });

            migrationBuilder.CreateTable(
                name: "FormularioResposta",
                columns: table => new
                {
                    FormularioRespostaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FormularioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EducandoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Authorized = table.Column<bool>(type: "bit", nullable: false),
                    DateLimit = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ResponseDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormularioResposta", x => x.FormularioRespostaId);
                    table.ForeignKey(
                        name: "FK_FormularioResposta_Educando_EducandoId",
                        column: x => x.EducandoId,
                        principalTable: "Educando",
                        principalColumn: "EducandoId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FormularioResposta_Formulario_FormularioId",
                        column: x => x.FormularioId,
                        principalTable: "Formulario",
                        principalColumn: "FormularioId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Agrupamento_ContaId",
                table: "Agrupamento",
                column: "ContaId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Atividade_AtlId",
                table: "Atividade",
                column: "AtlId");

            migrationBuilder.CreateIndex(
                name: "IX_AtividadeRecord_AtlId",
                table: "AtividadeRecord",
                column: "AtlId");

            migrationBuilder.CreateIndex(
                name: "IX_ATL_AgrupamentoId",
                table: "ATL",
                column: "AgrupamentoId");

            migrationBuilder.CreateIndex(
                name: "IX_ATLAdmin_AtlId",
                table: "ATLAdmin",
                column: "AtlId");

            migrationBuilder.CreateIndex(
                name: "IX_ATLAdmin_ContaId",
                table: "ATLAdmin",
                column: "ContaId");

            migrationBuilder.CreateIndex(
                name: "IX_ContaAdministrativa_AtlId",
                table: "ContaAdministrativa",
                column: "AtlId");

            migrationBuilder.CreateIndex(
                name: "IX_ContaAdministrativa_UserId",
                table: "ContaAdministrativa",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CoordATL_AtlId",
                table: "CoordATL",
                column: "AtlId");

            migrationBuilder.CreateIndex(
                name: "IX_CoordATL_ContaId",
                table: "CoordATL",
                column: "ContaId");

            migrationBuilder.CreateIndex(
                name: "IX_Educando_AtlId",
                table: "Educando",
                column: "AtlId");

            migrationBuilder.CreateIndex(
                name: "IX_Educando_EncarregadoId",
                table: "Educando",
                column: "EncarregadoId");

            migrationBuilder.CreateIndex(
                name: "IX_EducandoRecord_AtlId",
                table: "EducandoRecord",
                column: "AtlId");

            migrationBuilder.CreateIndex(
                name: "IX_EducandoResponsavel_EducandoId",
                table: "EducandoResponsavel",
                column: "EducandoId");

            migrationBuilder.CreateIndex(
                name: "IX_EducandoSaude_EducandoId",
                table: "EducandoSaude",
                column: "EducandoId");

            migrationBuilder.CreateIndex(
                name: "IX_EncarregadoEducacao_UserId",
                table: "EncarregadoEducacao",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Formulario_AtividadeId",
                table: "Formulario",
                column: "AtividadeId");

            migrationBuilder.CreateIndex(
                name: "IX_Formulario_AtlId",
                table: "Formulario",
                column: "AtlId");

            migrationBuilder.CreateIndex(
                name: "IX_Formulario_VisitaEstudoId",
                table: "Formulario",
                column: "VisitaEstudoId");

            migrationBuilder.CreateIndex(
                name: "IX_FormularioRecord_AtlId",
                table: "FormularioRecord",
                column: "AtlId");

            migrationBuilder.CreateIndex(
                name: "IX_FormularioResposta_EducandoId",
                table: "FormularioResposta",
                column: "EducandoId");

            migrationBuilder.CreateIndex(
                name: "IX_FormularioResposta_FormularioId",
                table: "FormularioResposta",
                column: "FormularioId");

            migrationBuilder.CreateIndex(
                name: "IX_FormularioRespostaRecord_FormularioRecordId",
                table: "FormularioRespostaRecord",
                column: "FormularioRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_FuncionarioRecord_AtlId",
                table: "FuncionarioRecord",
                column: "AtlId");

            migrationBuilder.CreateIndex(
                name: "IX_Notificacoes_UserId",
                table: "Notificacoes",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Recibo_AtlId",
                table: "Recibo",
                column: "AtlId");

            migrationBuilder.CreateIndex(
                name: "IX_ReciboResposta_EducandoId",
                table: "ReciboResposta",
                column: "EducandoId");

            migrationBuilder.CreateIndex(
                name: "IX_ReciboResposta_ReciboId",
                table: "ReciboResposta",
                column: "ReciboId");

            migrationBuilder.CreateIndex(
                name: "IX_Refeicao_AtlId",
                table: "Refeicao",
                column: "AtlId");

            migrationBuilder.CreateIndex(
                name: "IX_RefeicaoRecord_AtlId",
                table: "RefeicaoRecord",
                column: "AtlId");

            migrationBuilder.CreateIndex(
                name: "IX_VisitaEstudo_AtlId",
                table: "VisitaEstudo",
                column: "AtlId");

            migrationBuilder.CreateIndex(
                name: "IX_VisitaEstudoRecord_AtlId",
                table: "VisitaEstudoRecord",
                column: "AtlId");

            migrationBuilder.AddForeignKey(
                name: "FK_Agrupamento_ContaAdministrativa_ContaId",
                table: "Agrupamento",
                column: "ContaId",
                principalTable: "ContaAdministrativa",
                principalColumn: "ContaId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Agrupamento_ContaAdministrativa_ContaId",
                table: "Agrupamento");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "AtividadeRecord");

            migrationBuilder.DropTable(
                name: "ATLAdmin");

            migrationBuilder.DropTable(
                name: "CoordATL");

            migrationBuilder.DropTable(
                name: "EducandoRecord");

            migrationBuilder.DropTable(
                name: "EducandoResponsavel");

            migrationBuilder.DropTable(
                name: "EducandoSaude");

            migrationBuilder.DropTable(
                name: "FormularioResposta");

            migrationBuilder.DropTable(
                name: "FormularioRespostaRecord");

            migrationBuilder.DropTable(
                name: "FuncionarioRecord");

            migrationBuilder.DropTable(
                name: "Notificacoes");

            migrationBuilder.DropTable(
                name: "ReciboResposta");

            migrationBuilder.DropTable(
                name: "Refeicao");

            migrationBuilder.DropTable(
                name: "RefeicaoRecord");

            migrationBuilder.DropTable(
                name: "VisitaEstudoRecord");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Formulario");

            migrationBuilder.DropTable(
                name: "FormularioRecord");

            migrationBuilder.DropTable(
                name: "Educando");

            migrationBuilder.DropTable(
                name: "Recibo");

            migrationBuilder.DropTable(
                name: "Atividade");

            migrationBuilder.DropTable(
                name: "VisitaEstudo");

            migrationBuilder.DropTable(
                name: "EncarregadoEducacao");

            migrationBuilder.DropTable(
                name: "ContaAdministrativa");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "ATL");

            migrationBuilder.DropTable(
                name: "Agrupamento");
        }
    }
}
