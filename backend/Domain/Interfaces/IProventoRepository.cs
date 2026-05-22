using Domain.Entities;

namespace Domain.Interfaces;

public interface IProventoRepository
{
    Task<Provento?> GetByIdAsync(Guid id);
    Task<IEnumerable<Provento>> GetByAtivoIdAsync(Guid ativoId);
    Task<IEnumerable<Provento>> GetAllAsync(Guid carteiraId, Guid? ativoId = null, DateTime? dataInicio = null, DateTime? dataFim = null);
    Task<bool> ExistsSimilarAsync(Guid carteiraId, string ticker, DateTime dataPagamento, decimal valorPorCota, decimal quantidade, Domain.Enums.TipoProvento tipo);
    Task AddAsync(Provento provento);
    Task DeleteAsync(Guid id);
}