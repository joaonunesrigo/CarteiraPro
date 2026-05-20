using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.DataBase;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class OperacaoRepository : IOperacaoRepository
{
    private readonly AppDbContext _context;

    public OperacaoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Operacao?> GetByIdAsync(Guid id)
    {
        return await _context.Operacoes.FindAsync(id);
    }

    public async Task<IEnumerable<Operacao>> GetByAtivoIdAsync(Guid ativoId)
    {
        return await _context.Operacoes
            .Where(o => o.AtivoId == ativoId)
            .OrderBy(o => o.Data)
            .ThenBy(o => o.DataCadastro)
            .ThenBy(o => o.Id)
            .ToListAsync();
    }

    public async Task<IEnumerable<Operacao>> GetAllAsync()
    {
        return await _context.Operacoes
            .OrderBy(o => o.Data)
            .ThenBy(o => o.DataCadastro)
            .ThenBy(o => o.Id)
            .ToListAsync();
    }

    public async Task AddAsync(Operacao operacao)
    {
        await _context.Operacoes.AddAsync(operacao);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Operacao operacao)
    {
        _context.Operacoes.Remove(operacao);
        await _context.SaveChangesAsync();
    }
}
