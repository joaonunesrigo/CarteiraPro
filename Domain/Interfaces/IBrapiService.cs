namespace Domain.Interfaces;

public interface IBrapiService
{
    Task<decimal> GetCotacaoAsync(string ticker);
}