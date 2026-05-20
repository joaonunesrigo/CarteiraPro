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
    public DbSet<Operacao> Operacoes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Ativo>(entity =>
        {
            entity.HasKey(a => a.Id);
            entity.Property(a => a.Ticker).IsRequired().HasMaxLength(10);
            entity.Property(a => a.Nome).IsRequired().HasMaxLength(100);
        });

        modelBuilder.Entity<Operacao>(entity =>
        {
            entity.HasKey(o => o.Id);
            entity.Property(o => o.Quantidade).HasPrecision(18, 8);
            entity.Property(o => o.PrecoUnitario).HasPrecision(18, 6);
            entity.Property(o => o.Taxas).HasPrecision(18, 2);
            entity.Property(o => o.Observacao).HasMaxLength(500);
            entity.HasOne<Ativo>()
                .WithMany()
                .HasForeignKey(o => o.AtivoId)
                .OnDelete(DeleteBehavior.Cascade);
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