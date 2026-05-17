using Domain.Entities;

namespace Domain.Interfaces;

public interface IAtivoRepository
{
    Task<Ativo?> GetByIdAsync(Guid id);
    Task<IEnumerable<Ativo>> GetAllAsync();
    Task AddAsync(Ativo ativo);
    Task UpdateAsync(Ativo ativo);
    Task DeleteAsync(Guid id);
}