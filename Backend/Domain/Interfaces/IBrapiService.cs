namespace Domain.Interfaces;

public interface IBrapiService
{
    Task<decimal> GetCotacaoAsync(string ticker);

    Task<string> GetNomeAtivoAsync(string ticker);

}