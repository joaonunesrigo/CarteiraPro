using Domain.Entities;

namespace Domain.Interfaces;

public interface IOperacaoRepository
{
    Task<Operacao?> GetByIdAsync(Guid id);
    Task<IEnumerable<Operacao>> GetByAtivoIdAsync(Guid ativoId);
    Task<IEnumerable<Operacao>> GetAllAsync(Guid carteiraId);
    Task AddAsync(Operacao operacao);
    Task DeleteAsync(Operacao operacao);
}
