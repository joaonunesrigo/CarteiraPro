using Application.Dtos;
using Application.Services.Carteiras;
using Domain.Interfaces;

namespace Application.Services.Ativos;

public class PreviewImportacaoB3Service
{
    private readonly IB3PosicaoParser _parser;
    private readonly IAtivoRepository _ativoRepository;
    private readonly GetCarteiraAtualService _getCarteiraAtual;

    public PreviewImportacaoB3Service(
        IB3PosicaoParser parser,
        IAtivoRepository ativoRepository,
        GetCarteiraAtualService getCarteiraAtual)
    {
        _parser = parser;
        _ativoRepository = ativoRepository;
        _getCarteiraAtual = getCarteiraAtual;
    }

    public async Task<IReadOnlyList<LinhaImportacaoB3Dto>> ExecuteAsync(Stream arquivo, Guid? carteiraId = null)
    {
        var linhas = _parser.Parse(arquivo);
        var carteira = await _getCarteiraAtual.ExecuteAsync(carteiraId);
        var tickersCadastrados = await ObterTickersCadastradosAsync(carteira.Id);

        return linhas
            .Select(l => new LinhaImportacaoB3Dto(
                l.Ticker,
                l.Produto,
                l.Quantidade,
                l.PrecoFechamento,
                PrecoMedio: null,
                l.Tipo,
                tickersCadastrados.Contains(l.Ticker),
                l.OrigemAba))
            .ToList();
    }

    private async Task<HashSet<string>> ObterTickersCadastradosAsync(Guid carteiraId)
    {
        var ativos = await _ativoRepository.GetAllAsync(carteiraId);
        return ativos
            .Select(a => a.Ticker.ToUpperInvariant())
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
    }
}
