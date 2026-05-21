using Application.Exceptions;
using Domain.Interfaces;

namespace Application.Services.Ativos;

public class GetCotacaoAtivoService
{
    private readonly ICotacoesCache _cotacoesCache;

    public GetCotacaoAtivoService(ICotacoesCache cotacoesCache)
    {
        _cotacoesCache = cotacoesCache;
    }

    public async Task<decimal> ExecuteAsync(string ticker)
    {
        var tickerNormalizado = ticker.Trim().ToUpper();
        var cotacoes = await _cotacoesCache.ObterQuotesAsync([tickerNormalizado]);
        var quote = cotacoes.TryGetValue(tickerNormalizado, out var cotacao)
            ? cotacao
            : null;

        if (quote is null)
            throw new TickerInvalidoException(tickerNormalizado);

        return quote.Cotacao;
    }
}
