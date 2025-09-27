using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProgramaYA.Models;

namespace ProgramaYA.Areas.Identity.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Curso> Cursos { get; set; }
    public DbSet<Capitulo> Capitulos { get; set; }
    public DbSet<Suscripcion> Suscripciones { get; set; }
    public DbSet<Pago> Pagos { get; set; }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        ConfigureCurso(builder);
    }

    private void ConfigureCurso(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Capitulo>()
                    .HasOne(h => h.Curso)
                    .WithMany(h => h.Capitulos)
                    .HasForeignKey(fk => fk.CursoId)
                    .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Pago>()
                    .HasOne(h => h.Suscripcion)
                    .WithOne(h => h.Pago)
                    .HasForeignKey<Pago>(fk => fk.Id)
                    .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Suscripcion>()
                    .HasOne(h => h.Usuario)
                    .WithMany(h => h.Suscripciones)
                    .HasForeignKey(fk => fk.UsuarioId)
                    .OnDelete(DeleteBehavior.Restrict);
    }
}
