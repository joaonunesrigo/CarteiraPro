using Application.Exceptions;
using Application.Services.Carteiras;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;

namespace Application.Services.Ativos;

public class AddAtivoService
{
    private readonly IBrapiService _brapiService;
    private readonly IAtivoRepository _ativoRepository;
    private readonly IOperacaoRepository _operacaoRepository;
    private readonly ICurrentUser _currentUser;
    private readonly GetCarteiraAtualService _getCarteiraAtual;

    public AddAtivoService(
        IAtivoRepository ativoRepository,
        IBrapiService brapiService,
        IOperacaoRepository operacaoRepository,
        ICurrentUser currentUser,
        GetCarteiraAtualService getCarteiraAtual)
    {
        _ativoRepository = ativoRepository;
        _brapiService = brapiService;
        _operacaoRepository = operacaoRepository;
        _currentUser = currentUser;
        _getCarteiraAtual = getCarteiraAtual;
    }

    public async Task ExecuteAsync(Guid? carteiraId, string ticker, decimal precoMedio, decimal quantidade, TipoAtivo tipo)
    {
        var carteira = await _getCarteiraAtual.ExecuteAsync(carteiraId);
        var tickerNormalizado = ticker.Trim().ToUpper();

        if (string.IsNullOrWhiteSpace(tickerNormalizado))
            throw new TickerInvalidoException(ticker);

        if (precoMedio <= 0)
            throw new OperacaoInvalidaException("Preço médio deve ser maior que zero.");

        if (quantidade <= 0)
            throw new OperacaoInvalidaException("Quantidade deve ser maior que zero.");

        var ativoExistente = await _ativoRepository.GetByTickerAsync(tickerNormalizado, carteira.Id);
        if (ativoExistente is not null)
            throw new TickerJaCadastradoException(tickerNormalizado);

        var quote = await _brapiService.ObterQuoteAsync(tickerNormalizado);
        if (quote is null)
            throw new TickerInvalidoException(tickerNormalizado);

        var usuarioId = _currentUser.UsuarioId
            ?? throw new InvalidOperationException("Usuário autenticado não encontrado.");

        var ativo = new Ativo(usuarioId, carteira.Id, tickerNormalizado, quote.Nome, tipo);
        await _ativoRepository.AddAsync(ativo);

        var operacaoInicial = new Operacao(
            usuarioId,
            carteira.Id,
            ativo.Id,
            Domain.Enums.TipoOperacao.Compra,
            DateTime.UtcNow,
            quantidade,
            precoMedio);

        await _operacaoRepository.AddAsync(operacaoInicial);
    }
}
