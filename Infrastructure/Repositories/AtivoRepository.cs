using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.DataBase;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class AtivoRepository : IAtivoRepository
{
    private readonly AppDbContext _context;

    public AtivoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Ativo> GetByIdAsync(Guid id)
    {
        return await _context.Ativos.FindAsync(id);
    }

    public async Task<IEnumerable<Ativo>> GetAllAsync()
    {
        return await _context.Ativos.ToListAsync();
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
}