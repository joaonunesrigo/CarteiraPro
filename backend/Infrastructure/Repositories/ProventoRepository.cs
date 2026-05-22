using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Infrastructure.DataBase;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ProventoRepository : IProventoRepository
{
    private readonly AppDbContext _context;
    private readonly ICurrentUser _currentUser;

    public ProventoRepository(AppDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    private Guid UsuarioId => _currentUser.UsuarioId
        ?? throw new InvalidOperationException("Usuário autenticado não encontrado.");

    public async Task<Provento?> GetByIdAsync(Guid id)
    {
        return await _context.Proventos.FirstOrDefaultAsync(p => p.Id == id && p.UsuarioId == UsuarioId);
    }

    public async Task<IEnumerable<Provento>> GetByAtivoIdAsync(Guid ativoId)
    {
        return await _context.Proventos
            .Where(p => p.UsuarioId == UsuarioId && p.AtivoId == ativoId)
            .OrderByDescending(p => p.DataPagamento)
            .ToListAsync();
    }

    public async Task<IEnumerable<Provento>> GetAllAsync(Guid carteiraId, Guid? ativoId = null, DateTime? dataInicio = null, DateTime? dataFim = null)
    {
        var query = _context.Proventos
            .Where(p => p.UsuarioId == UsuarioId && p.CarteiraId == carteiraId)
            .AsQueryable();

        if (ativoId.HasValue)
            query = query.Where(p => p.AtivoId == ativoId.Value);

        if (dataInicio.HasValue)
            query = query.Where(p => p.DataPagamento >= dataInicio.Value);

        if (dataFim.HasValue)
            query = query.Where(p => p.DataPagamento <= dataFim.Value);

        return await query
            .OrderByDescending(p => p.DataPagamento)
            .ToListAsync();
    }

    public async Task<bool> ExistsSimilarAsync(
        Guid carteiraId,
        string ticker,
        DateTime dataPagamento,
        decimal valorPorCota,
        decimal quantidade,
        TipoProvento tipo)
    {
        var data = dataPagamento.Date;
        var tickerNormalizado = ticker.Trim().ToUpperInvariant();
        var valorReferencia = Math.Round(valorPorCota, 6, MidpointRounding.AwayFromZero);
        const decimal tolerancia = 0.0000005m;

        return await _context.Proventos.AnyAsync(p =>
            p.UsuarioId == UsuarioId
            && p.CarteiraId == carteiraId
            && p.Ticker == tickerNormalizado
            && p.DataPagamento.Date == data
            && p.Quantidade == quantidade
            && p.Tipo == tipo
            && p.ValorPorCota >= valorReferencia - tolerancia
            && p.ValorPorCota <= valorReferencia + tolerancia);
    }

    public async Task AddAsync(Provento provento)
    {
        await _context.Proventos.AddAsync(provento);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var provento = await GetByIdAsync(id);
        if (provento is null)
            return;

        _context.Proventos.Remove(provento);
        await _context.SaveChangesAsync();
    }
}
