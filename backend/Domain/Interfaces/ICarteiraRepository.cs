using Domain.Entities;

namespace Domain.Interfaces;

public interface ICarteiraRepository
{
    Task<Carteira?> GetByIdAsync(Guid id);
    Task<Carteira?> GetPadraoAsync();
    Task<IEnumerable<Carteira>> GetAllAsync();
    Task AddAsync(Carteira carteira);
    Task UpdateAsync(Carteira carteira);
    Task DeleteAsync(Carteira carteira);
    Task<bool> NomeExisteAsync(string nome, Guid? ignorarCarteiraId = null);
}
