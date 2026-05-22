using Domain.Entities;

namespace Domain.Interfaces;

public interface IAtivoRepository
{
    Task<Ativo?> GetByIdAsync(Guid id);
    Task<Ativo?> GetByTickerAsync(string ticker, Guid carteiraId);
    Task<IEnumerable<Ativo>> GetAllAsync(Guid carteiraId);
    Task AddAsync(Ativo ativo);
    Task UpdateAsync(Ativo ativo);
    Task DeleteAsync(Guid id);
    Task<int> DeleteAllAsync(Guid carteiraId);
}