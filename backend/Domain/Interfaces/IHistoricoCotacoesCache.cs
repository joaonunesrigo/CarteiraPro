using Domain.Models;

namespace Domain.Interfaces;

public interface IHistoricoCotacoesCache
{
    Task<IReadOnlyDictionary<string, IReadOnlyList<HistoricoCotacaoMensal>>> ObterHistoricoMensalAsync(
        IEnumerable<string> tickers,
        int meses);
}
