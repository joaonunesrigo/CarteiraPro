using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.DataBase;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class AtivoRepository : IAtivoRepository
{
    private readonly AppDbContext _context;
    private readonly ICurrentUser _currentUser;

    public AtivoRepository(AppDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    private Guid UsuarioId => _currentUser.UsuarioId
        ?? throw new InvalidOperationException("Usuário autenticado não encontrado.");

    public async Task<Ativo?> GetByIdAsync(Guid id)
    {
        return await _context.Ativos.FirstOrDefaultAsync(a => a.Id == id && a.UsuarioId == UsuarioId);
    }

    public async Task<Ativo?> GetByTickerAsync(string ticker)
    {
        var tickerNormalizado = ticker.Trim().ToUpper();
        return await _context.Ativos
            .FirstOrDefaultAsync(a => a.UsuarioId == UsuarioId && a.Ticker == tickerNormalizado);
    }

    public async Task<IEnumerable<Ativo>> GetAllAsync()
    {
        return await _context.Ativos
            .Where(a => a.UsuarioId == UsuarioId)
            .ToListAsync();
    }

    public async Task AddAsync(Ativo ativo)
    {
        await _context.Ativos.AddAsync(ativo);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Ativo ativo)
    {
        _context.Ativos.Update(ativo);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var ativo = await GetByIdAsync(id);
        if (ativo != null)
        {
            _context.Ativos.Remove(ativo);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<int> DeleteAllAsync()
    {
        var ativos = await _context.Ativos
            .Where(a => a.UsuarioId == UsuarioId)
            .ToListAsync();
        if (ativos.Count == 0)
            return 0;

        _context.Ativos.RemoveRange(ativos);
        await _context.SaveChangesAsync();
        return ativos.Count;
    }
}