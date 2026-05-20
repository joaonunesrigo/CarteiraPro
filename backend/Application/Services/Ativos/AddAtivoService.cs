using Application.Exceptions;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;

namespace Application.Services.Ativos;

public class AddAtivoService
{
    private readonly IBrapiService _brapiService;
    private readonly IAtivoRepository _ativoRepository;

    public AddAtivoService(IAtivoRepository ativoRepository, IBrapiService brapiService)
    {
        _ativoRepository = ativoRepository;
        _brapiService = brapiService;
    }

    public async Task ExecuteAsync(string ticker, decimal precoMedio, decimal quantidade, TipoAtivo tipo)
    {
        var tickerNormalizado = ticker.Trim().ToUpper();

        if (string.IsNullOrWhiteSpace(tickerNormalizado))
            throw new TickerInvalidoException(ticker);

        var ativoExistente = await _ativoRepository.GetByTickerAsync(tickerNormalizado);
        if (ativoExistente is not null)
            throw new TickerJaCadastradoException(tickerNormalizado);

        var quote = await _brapiService.ObterQuoteAsync(tickerNormalizado);
        if (quote is null)
            throw new TickerInvalidoException(tickerNormalizado);

        var ativo = new Ativo(tickerNormalizado, quote.Nome, precoMedio, quantidade, tipo);
        await _ativoRepository.AddAsync(ativo);
    }
}
