﻿// <auto-generated />
using System;
using ATLManager.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace ATLManager.Migrations
{
    [DbContext(typeof(ATLManagerAuthContext))]
    [Migration("20230411140028_Update_VisitaEstudo_Location")]
    partial class Update_VisitaEstudo_Location
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.15")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("ATLManager.Areas.Identity.Data.ATLManagerUser", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("nvarchar(100)");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers", (string)null);
                });

            modelBuilder.Entity("ATLManager.Models.Agrupamento", b =>
                {
                    b.Property<Guid>("AgrupamentoID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("ContaId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Location")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<string>("LogoPicture")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("NIPC")
                        .IsRequired()
                        .HasMaxLength(9)
                        .HasColumnType("nvarchar(9)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.HasKey("AgrupamentoID");

                    b.HasIndex("ContaId");

                    b.ToTable("Agrupamento");
                });

            modelBuilder.Entity("ATLManager.Models.ATL", b =>
                {
                    b.Property<Guid>("AtlId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<Guid?>("AgrupamentoId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("City")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<string>("LogoPicture")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("NIPC")
                        .HasMaxLength(9)
                        .HasColumnType("nvarchar(9)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("PostalCode")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("AtlId");

                    b.HasIndex("AgrupamentoId");

                    b.ToTable("ATL");
                });

            modelBuilder.Entity("ATLManager.Models.ATLAdmin", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("AtlId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ContaId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("AtlId");

                    b.HasIndex("ContaId");

                    b.ToTable("ATLAdmin");
                });

            modelBuilder.Entity("ATLManager.Models.ContaAdministrativa", b =>
                {
                    b.Property<Guid>("ContaId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("AtlId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("CC")
                        .IsRequired()
                        .HasMaxLength(9)
                        .HasColumnType("nvarchar(9)");

                    b.Property<DateTime>("DateOfBirth")
                        .HasColumnType("datetime2");

                    b.Property<string>("ProfilePicture")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("ContaId");

                    b.HasIndex("AtlId");

                    b.HasIndex("UserId");

                    b.ToTable("ContaAdministrativa");
                });

            modelBuilder.Entity("ATLManager.Models.CoordATL", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("AtlId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ContaId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("AtlId");

                    b.HasIndex("ContaId");

                    b.ToTable("CoordATL");
                });

            modelBuilder.Entity("ATLManager.Models.Educando", b =>
                {
                    b.Property<Guid>("EducandoId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Apelido")
                        .IsRequired()
                        .HasColumnType("nvarchar(100)");

                    b.Property<Guid>("AtlId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("BoletimVacinas")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CC")
                        .IsRequired()
                        .HasMaxLength(9)
                        .HasColumnType("nvarchar(9)");

                    b.Property<string>("DeclaracaoMedica")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("EncarregadoId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Genero")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("ProfilePicture")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("EducandoId");

                    b.HasIndex("AtlId");

                    b.HasIndex("EncarregadoId");

                    b.ToTable("Educando");
                });

            modelBuilder.Entity("ATLManager.Models.EducandoSaude", b =>
                {
                    b.Property<Guid>("EducandoSaudeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Allergies")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("BloodType")
                        .HasColumnType("nvarchar(3)");

                    b.Property<string>("Diseases")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("EducandoId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("EmergencyContact")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("InsuranceName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("InsuranceNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("MedicalHistory")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Medication")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("EducandoSaudeId");

                    b.HasIndex("EducandoId");

                    b.ToTable("EducandoSaude");
                });

            modelBuilder.Entity("ATLManager.Models.EncarregadoEducacao", b =>
                {
                    b.Property<Guid>("EncarregadoId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("City")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<string>("FullName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("NIF")
                        .HasMaxLength(9)
                        .HasColumnType("int");

                    b.Property<int>("Phone")
                        .HasColumnType("int");

                    b.Property<string>("PostalCode")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ProfilePicture")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("EncarregadoId");

                    b.HasIndex("UserId");

                    b.ToTable("EncarregadoEducacao");
                });

            modelBuilder.Entity("ATLManager.Models.Formulario", b =>
                {
                    b.Property<Guid>("FormularioId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("AtlId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("DateLimit")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(1000)
                        .HasColumnType("nvarchar(1000)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("VisitaEstudoId")
                        .IsRequired()
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("FormularioId");

                    b.HasIndex("AtlId");

                    b.HasIndex("VisitaEstudoId");

                    b.ToTable("Formulario");
                });

            modelBuilder.Entity("ATLManager.Models.FormularioResposta", b =>
                {
                    b.Property<Guid>("FormularioRespostaId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("Authorized")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("DateLimit")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("EducandoId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("FormularioId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("ResponseDate")
                        .HasColumnType("datetime2");

                    b.HasKey("FormularioRespostaId");

                    b.HasIndex("EducandoId");

                    b.HasIndex("FormularioId");

                    b.ToTable("FormularioResposta");
                });

            modelBuilder.Entity("ATLManager.Models.Refeicao", b =>
                {
                    b.Property<Guid>("RefeicaoId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("AGSat")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Acucar")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("AtlId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Categoria")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<DateTime>("Data")
                        .HasColumnType("datetime2");

                    b.Property<string>("Descricao")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("HidratosCarbono")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Lipidos")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<string>("Picture")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Proteina")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Sal")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("VR")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ValorEnergetico")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("RefeicaoId");

                    b.HasIndex("AtlId");

                    b.ToTable("Refeicao");
                });

            modelBuilder.Entity("ATLManager.Models.VisitaEstudo", b =>
                {
                    b.Property<Guid>("VisitaEstudoID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("AtlId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<string>("Descripton")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Location")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Picture")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("VisitaEstudoID");

                    b.HasIndex("AtlId");

                    b.ToTable("VisitaEstudo");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("ProviderKey")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("RoleId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("LoginProvider")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("Name")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens", (string)null);
                });

            modelBuilder.Entity("ATLManager.Models.Agrupamento", b =>
                {
                    b.HasOne("ATLManager.Models.ContaAdministrativa", "ContaAdministrativa")
                        .WithMany()
                        .HasForeignKey("ContaId");

                    b.Navigation("ContaAdministrativa");
                });

            modelBuilder.Entity("ATLManager.Models.ATL", b =>
                {
                    b.HasOne("ATLManager.Models.Agrupamento", "Agrupamento")
                        .WithMany()
                        .HasForeignKey("AgrupamentoId");

                    b.Navigation("Agrupamento");
                });

            modelBuilder.Entity("ATLManager.Models.ATLAdmin", b =>
                {
                    b.HasOne("ATLManager.Models.ATL", "Atl")
                        .WithMany()
                        .HasForeignKey("AtlId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ATLManager.Models.ContaAdministrativa", "ContaAdministrativa")
                        .WithMany()
                        .HasForeignKey("ContaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Atl");

                    b.Navigation("ContaAdministrativa");
                });

            modelBuilder.Entity("ATLManager.Models.ContaAdministrativa", b =>
                {
                    b.HasOne("ATLManager.Models.ATL", "Atl")
                        .WithMany()
                        .HasForeignKey("AtlId");

                    b.HasOne("ATLManager.Areas.Identity.Data.ATLManagerUser", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Atl");

                    b.Navigation("User");
                });

            modelBuilder.Entity("ATLManager.Models.CoordATL", b =>
                {
                    b.HasOne("ATLManager.Models.ATL", "Atl")
                        .WithMany()
                        .HasForeignKey("AtlId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ATLManager.Models.ContaAdministrativa", "ContaAdministrativa")
                        .WithMany()
                        .HasForeignKey("ContaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Atl");

                    b.Navigation("ContaAdministrativa");
                });

            modelBuilder.Entity("ATLManager.Models.Educando", b =>
                {
                    b.HasOne("ATLManager.Models.ATL", "Atl")
                        .WithMany()
                        .HasForeignKey("AtlId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ATLManager.Models.EncarregadoEducacao", "Encarregado")
                        .WithMany()
                        .HasForeignKey("EncarregadoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Atl");

                    b.Navigation("Encarregado");
                });

            modelBuilder.Entity("ATLManager.Models.EducandoSaude", b =>
                {
                    b.HasOne("ATLManager.Models.Educando", "Educando")
                        .WithMany()
                        .HasForeignKey("EducandoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Educando");
                });

            modelBuilder.Entity("ATLManager.Models.EncarregadoEducacao", b =>
                {
                    b.HasOne("ATLManager.Areas.Identity.Data.ATLManagerUser", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("ATLManager.Models.Formulario", b =>
                {
                    b.HasOne("ATLManager.Models.ATL", "Atl")
                        .WithMany()
                        .HasForeignKey("AtlId");

                    b.HasOne("ATLManager.Models.VisitaEstudo", "VisitaEstudo")
                        .WithMany()
                        .HasForeignKey("VisitaEstudoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Atl");

                    b.Navigation("VisitaEstudo");
                });

            modelBuilder.Entity("ATLManager.Models.FormularioResposta", b =>
                {
                    b.HasOne("ATLManager.Models.Educando", "Educando")
                        .WithMany()
                        .HasForeignKey("EducandoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ATLManager.Models.Formulario", "Formulario")
                        .WithMany()
                        .HasForeignKey("FormularioId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Educando");

                    b.Navigation("Formulario");
                });

            modelBuilder.Entity("ATLManager.Models.Refeicao", b =>
                {
                    b.HasOne("ATLManager.Models.ATL", "Atl")
                        .WithMany()
                        .HasForeignKey("AtlId");

                    b.Navigation("Atl");
                });

            modelBuilder.Entity("ATLManager.Models.VisitaEstudo", b =>
                {
                    b.HasOne("ATLManager.Models.ATL", "Atl")
                        .WithMany()
                        .HasForeignKey("AtlId");

                    b.Navigation("Atl");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("ATLManager.Areas.Identity.Data.ATLManagerUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("ATLManager.Areas.Identity.Data.ATLManagerUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ATLManager.Areas.Identity.Data.ATLManagerUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("ATLManager.Areas.Identity.Data.ATLManagerUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
