using Application.Exceptions;
using Domain.Interfaces;

namespace Application.Services.Ativos;

public class GetCotacaoAtivoService
{
    private readonly IBrapiService _brapiService;

    public GetCotacaoAtivoService(IBrapiService brapiService)
    {
        _brapiService = brapiService;
    }

    public async Task<decimal> ExecuteAsync(string ticker)
    {
        var tickerNormalizado = ticker.Trim().ToUpper();
        var quote = await _brapiService.ObterQuoteAsync(tickerNormalizado);

        if (quote is null)
            throw new TickerInvalidoException(tickerNormalizado);

        return quote.Cotacao;
    }
}
