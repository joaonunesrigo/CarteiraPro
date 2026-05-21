using Domain.Models;

namespace Domain.Interfaces;

public interface ICotacoesCache
{
    Task<IReadOnlyDictionary<string, BrapiQuote>> ObterQuotesAsync(IEnumerable<string> tickers);
    Task AtualizarQuotesAsync(IEnumerable<string> tickers);
}
