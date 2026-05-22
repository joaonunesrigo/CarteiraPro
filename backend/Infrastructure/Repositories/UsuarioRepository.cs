using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.DataBase;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly AppDbContext _context;

    public UsuarioRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Usuario?> GetByIdAsync(Guid id)
    {
        return await _context.Usuarios.FindAsync(id);
    }

    public async Task<Usuario?> GetByEmailAsync(string email)
    {
        var emailNormalizado = email.Trim().ToLowerInvariant();
        return await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == emailNormalizado);
    }

    public async Task<bool> ExistsByEmailAsync(string email)
    {
        var emailNormalizado = email.Trim().ToLowerInvariant();
        return await _context.Usuarios.AnyAsync(u => u.Email == emailNormalizado);
    }

    public async Task AddAsync(Usuario usuario)
    {
        await _context.Usuarios.AddAsync(usuario);
        await _context.Carteiras.AddAsync(new Carteira(usuario.Id, "Minha Carteira", Domain.Enums.Moeda.BRL, padrao: true));
        await _context.SaveChangesAsync();
    }
}
