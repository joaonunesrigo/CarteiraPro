using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.DataBase;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class CarteiraRepository : ICarteiraRepository
{
    private readonly AppDbContext _context;
    private readonly ICurrentUser _currentUser;

    public CarteiraRepository(AppDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    private Guid UsuarioId => _currentUser.UsuarioId
        ?? throw new InvalidOperationException("Usuário autenticado não encontrado.");

    public async Task<Carteira?> GetByIdAsync(Guid id)
    {
        return await _context.Carteiras.FirstOrDefaultAsync(c => c.Id == id && c.UsuarioId == UsuarioId);
    }

    public async Task<Carteira?> GetPadraoAsync()
    {
        return await _context.Carteiras
            .Where(c => c.UsuarioId == UsuarioId)
            .OrderByDescending(c => c.Padrao)
            .ThenBy(c => c.DataCadastro)
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Carteira>> GetAllAsync()
    {
        return await _context.Carteiras
            .Where(c => c.UsuarioId == UsuarioId)
            .OrderByDescending(c => c.Padrao)
            .ThenBy(c => c.Nome)
            .ToListAsync();
    }

    public async Task AddAsync(Carteira carteira)
    {
        await _context.Carteiras.AddAsync(carteira);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Carteira carteira)
    {
        _context.Carteiras.Update(carteira);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Carteira carteira)
    {
        _context.Carteiras.Remove(carteira);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> NomeExisteAsync(string nome, Guid? ignorarCarteiraId = null)
    {
        var nomeNormalizado = nome.Trim().ToUpperInvariant();
        return await _context.Carteiras.AnyAsync(c =>
            c.UsuarioId == UsuarioId
            && c.Nome.ToUpper() == nomeNormalizado
            && (!ignorarCarteiraId.HasValue || c.Id != ignorarCarteiraId.Value));
    }
}
