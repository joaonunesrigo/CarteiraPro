using Application.Dtos;
using Domain.Interfaces;

namespace Application.Services.Ativos;

public class PreviewImportacaoB3Service
{
    private readonly IB3PosicaoParser _parser;
    private readonly IAtivoRepository _ativoRepository;

    public PreviewImportacaoB3Service(IB3PosicaoParser parser, IAtivoRepository ativoRepository)
    {
        _parser = parser;
        _ativoRepository = ativoRepository;
    }

    public async Task<IReadOnlyList<LinhaImportacaoB3Dto>> ExecuteAsync(Stream arquivo)
    {
        var linhas = _parser.Parse(arquivo);
        var tickersCadastrados = await ObterTickersCadastradosAsync();

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

    private async Task<HashSet<string>> ObterTickersCadastradosAsync()
    {
        var ativos = await _ativoRepository.GetAllAsync();
        return ativos
            .Select(a => a.Ticker.ToUpperInvariant())
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
    }
}
