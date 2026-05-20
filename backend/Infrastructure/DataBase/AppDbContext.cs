using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DataBase;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Ativo> Ativos { get; set; }
    public DbSet<Provento> Proventos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Ativo>(entity =>
        {
            entity.HasKey(a => a.Id);
            entity.Property(a => a.Ticker).IsRequired().HasMaxLength(10);
            entity.Property(a => a.Nome).IsRequired().HasMaxLength(100);
            entity.Property(a => a.PrecoMedio).HasPrecision(18, 2);
            entity.Property(a => a.Quantidade).HasPrecision(18, 8);
        });

        modelBuilder.Entity<Provento>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Ticker).IsRequired().HasMaxLength(10);
            entity.Property(p => p.ValorPorCota).HasPrecision(18, 6);
            entity.Property(p => p.Quantidade).HasPrecision(18, 8);
        });
    }
}