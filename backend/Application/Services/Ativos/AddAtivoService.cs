using Application.Exceptions;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;

namespace Application.Services.Ativos;

public class AddAtivoService
{
    private readonly IBrapiService _brapiService;
    private readonly IAtivoRepository _ativoRepository;
    private readonly IOperacaoRepository _operacaoRepository;

    public AddAtivoService(
        IAtivoRepository ativoRepository,
        IBrapiService brapiService,
        IOperacaoRepository operacaoRepository)
    {
        _ativoRepository = ativoRepository;
        _brapiService = brapiService;
        _operacaoRepository = operacaoRepository;
    }

    public async Task ExecuteAsync(string ticker, decimal precoMedio, decimal quantidade, TipoAtivo tipo)
    {
        var tickerNormalizado = ticker.Trim().ToUpper();

        if (string.IsNullOrWhiteSpace(tickerNormalizado))
            throw new TickerInvalidoException(ticker);

        if (precoMedio <= 0)
            throw new OperacaoInvalidaException("Preço médio deve ser maior que zero.");

        if (quantidade <= 0)
            throw new OperacaoInvalidaException("Quantidade deve ser maior que zero.");

        var ativoExistente = await _ativoRepository.GetByTickerAsync(tickerNormalizado);
        if (ativoExistente is not null)
            throw new TickerJaCadastradoException(tickerNormalizado);

        var quote = await _brapiService.ObterQuoteAsync(tickerNormalizado);
        if (quote is null)
            throw new TickerInvalidoException(tickerNormalizado);

        var ativo = new Ativo(tickerNormalizado, quote.Nome, tipo);
        await _ativoRepository.AddAsync(ativo);

        var operacaoInicial = new Operacao(
            ativo.Id,
            Domain.Enums.TipoOperacao.Compra,
            DateTime.UtcNow,
            quantidade,
            precoMedio);

        await _operacaoRepository.AddAsync(operacaoInicial);
    }
}
