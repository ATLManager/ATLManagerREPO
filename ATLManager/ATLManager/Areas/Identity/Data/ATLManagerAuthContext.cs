using ATLManager.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ATLManager.Models.Historicos;
using ATLManager.Models;

namespace ATLManager.Data;

public class ATLManagerAuthContext : IdentityDbContext<ATLManagerUser>
{
    public ATLManagerAuthContext(DbContextOptions<ATLManagerAuthContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
	}

    public DbSet<Agrupamento> Agrupamento { get; set; }
    public DbSet<ContaAdministrativa> ContaAdministrativa { get; set; }
    public DbSet<EncarregadoEducacao> EncarregadoEducacao { get; set; }
    public DbSet<ATL> ATL { get; set; }
    public DbSet<Refeicao> Refeicao { get; set; }
    public DbSet<Educando> Educando { get; set; }
    public DbSet<EducandoSaude> EducandoSaude { get; set; }
    public DbSet<EducandoResponsavel> EducandoResponsavel { get; set; }
    public DbSet<VisitaEstudo> VisitaEstudo { get; set; }
    public DbSet<VisitaEstudoRecord> VisitaEstudoRecord { get; set; }
    public DbSet<Formulario> Formulario { get; set; }
    public DbSet<FormularioResposta> FormularioResposta { get; set; }
    public DbSet<ATLAdmin> ATLAdmin { get; set; }
    public DbSet<CoordATL> CoordATL { get; set; }
    public DbSet<Atividade> Atividade { get; set; }
	public DbSet<AtividadeRecord> AtividadeRecord { get; set; }
    public DbSet<Recibo> Recibo { get; set; }
    public DbSet<ReciboResposta> ReciboResposta { get; set; }
	public DbSet<Notificacao> Notificacoes { get; set; }
	public DbSet<FuncionarioRecord> FuncionarioRecord { get; set; }
}
