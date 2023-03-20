using ATLManager.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
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
}
