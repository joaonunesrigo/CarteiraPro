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
    public DbSet<Usuario> Usuarios { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Nome).IsRequired().HasMaxLength(120);
            entity.Property(u => u.Email).IsRequired().HasMaxLength(180);
            entity.Property(u => u.SenhaHash).IsRequired().HasMaxLength(300);
            entity.HasIndex(u => u.Email).IsUnique();
        });

        modelBuilder.Entity<Ativo>(entity =>
        {
            entity.HasKey(a => a.Id);
            entity.Property(a => a.UsuarioId).IsRequired();
            entity.Property(a => a.Ticker).IsRequired().HasMaxLength(10);
            entity.Property(a => a.Nome).IsRequired().HasMaxLength(100);
            entity.HasIndex(a => new { a.UsuarioId, a.Ticker }).IsUnique();
            entity.HasOne<Usuario>()
                .WithMany()
                .HasForeignKey(a => a.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Operacao>(entity =>
        {
            entity.HasKey(o => o.Id);
            entity.Property(o => o.UsuarioId).IsRequired();
            entity.Property(o => o.Quantidade).HasPrecision(18, 8);
            entity.Property(o => o.PrecoUnitario).HasPrecision(18, 6);
            entity.Property(o => o.Taxas).HasPrecision(18, 2);
            entity.Property(o => o.Observacao).HasMaxLength(500);
            entity.HasOne<Ativo>()
                .WithMany()
                .HasForeignKey(o => o.AtivoId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne<Usuario>()
                .WithMany()
                .HasForeignKey(o => o.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Provento>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.UsuarioId).IsRequired();
            entity.Property(p => p.Ticker).IsRequired().HasMaxLength(10);
            entity.Property(p => p.ValorPorCota).HasPrecision(18, 6);
            entity.Property(p => p.Quantidade).HasPrecision(18, 8);
            entity.HasOne<Usuario>()
                .WithMany()
                .HasForeignKey(p => p.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}