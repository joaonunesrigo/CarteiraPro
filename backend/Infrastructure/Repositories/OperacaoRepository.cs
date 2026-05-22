using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.DataBase;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class OperacaoRepository : IOperacaoRepository
{
    private readonly AppDbContext _context;
    private readonly ICurrentUser _currentUser;

    public OperacaoRepository(AppDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    private Guid UsuarioId => _currentUser.UsuarioId
        ?? throw new InvalidOperationException("Usuário autenticado não encontrado.");

    public async Task<Operacao?> GetByIdAsync(Guid id)
    {
        return await _context.Operacoes.FirstOrDefaultAsync(o => o.Id == id && o.UsuarioId == UsuarioId);
    }

    public async Task<IEnumerable<Operacao>> GetByAtivoIdAsync(Guid ativoId)
    {
        return await _context.Operacoes
            .Where(o => o.UsuarioId == UsuarioId && o.AtivoId == ativoId)
            .OrderBy(o => o.Data)
            .ThenBy(o => o.DataCadastro)
            .ThenBy(o => o.Id)
            .ToListAsync();
    }

    public async Task<IEnumerable<Operacao>> GetAllAsync(Guid carteiraId)
    {
        return await _context.Operacoes
            .Where(o => o.UsuarioId == UsuarioId && o.CarteiraId == carteiraId)
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
