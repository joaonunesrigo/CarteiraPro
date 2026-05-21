using Domain.Models;

namespace Domain.Interfaces;

public interface IBrapiService
{
    Task<BrapiQuote?> ObterQuoteAsync(string ticker);
    Task<IReadOnlyDictionary<string, BrapiQuote>> ObterQuotesAsync(IEnumerable<string> tickers);
    Task<decimal> GetCotacaoAsync(string ticker);
    Task<string> GetNomeAtivoAsync(string ticker);
    Task<IReadOnlyList<HistoricoCotacaoMensal>> ObterHistoricoMensalAsync(string ticker, int meses);
}