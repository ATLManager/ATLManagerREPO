using ATLManager.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ATLManager.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        
        public DbSet<Conta> conta_utilizador { get; set; }
        

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
    }
}