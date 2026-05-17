using Domain.Entities;

namespace Domain.Interfaces;

public interface IProventoRepository
{
    Task<Provento> GetByIdAsync(Guid id);
    Task<IEnumerable<Provento>> GetByAtivoIdAsync(Guid ativoId);
    Task AddAsync(Provento provento);
    Task DeleteAsync(Guid id);
}