using Application.Dtos;
using Application.Services.Carteiras;
using Domain.Interfaces;

namespace Application.Services.Proventos;

public class PreviewImportacaoProventosB3Service
{
    private readonly IB3MovimentacaoParser _parser;
    private readonly IAtivoRepository _ativoRepository;
    private readonly IProventoRepository _proventoRepository;
    private readonly GetCarteiraAtualService _getCarteiraAtual;

    public PreviewImportacaoProventosB3Service(
        IB3MovimentacaoParser parser,
        IAtivoRepository ativoRepository,
        IProventoRepository proventoRepository,
        GetCarteiraAtualService getCarteiraAtual)
    {
        _parser = parser;
        _ativoRepository = ativoRepository;
        _proventoRepository = proventoRepository;
        _getCarteiraAtual = getCarteiraAtual;
    }

    public async Task<IReadOnlyList<LinhaImportacaoProventoB3Dto>> ExecuteAsync(Stream arquivo, Guid? carteiraId = null)
    {
        var linhas = _parser.Parse(arquivo);
        var carteira = await _getCarteiraAtual.ExecuteAsync(carteiraId);
        var ativos = (await _ativoRepository.GetAllAsync(carteira.Id))
            .ToDictionary(a => a.Ticker, StringComparer.OrdinalIgnoreCase);

        var preview = new List<LinhaImportacaoProventoB3Dto>();

        foreach (var linha in linhas)
        {
            var ativoCadastrado = ativos.TryGetValue(linha.Ticker, out var ativo);
            var jaImportado = await _proventoRepository.ExistsSimilarAsync(
                carteira.Id,
                linha.Ticker,
                linha.DataPagamento,
                linha.ValorPorCota,
                linha.Quantidade,
                linha.Tipo);

            preview.Add(new LinhaImportacaoProventoB3Dto(
                linha.Ticker,
                linha.Produto,
                linha.Movimentacao,
                linha.Quantidade,
                linha.ValorPorCota,
                linha.ValorTotal,
                linha.DataPagamento,
                linha.Tipo,
                ativoCadastrado,
                jaImportado,
                linha.OrigemAba));
        }

        return preview;
    }
}
