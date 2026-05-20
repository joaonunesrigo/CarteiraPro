using Application.Dtos;
using Domain.Interfaces;

namespace Application.Services.Proventos;

public class PreviewImportacaoProventosB3Service
{
    private readonly IB3MovimentacaoParser _parser;
    private readonly IAtivoRepository _ativoRepository;
    private readonly IProventoRepository _proventoRepository;

    public PreviewImportacaoProventosB3Service(
        IB3MovimentacaoParser parser,
        IAtivoRepository ativoRepository,
        IProventoRepository proventoRepository)
    {
        _parser = parser;
        _ativoRepository = ativoRepository;
        _proventoRepository = proventoRepository;
    }

    public async Task<IReadOnlyList<LinhaImportacaoProventoB3Dto>> ExecuteAsync(Stream arquivo)
    {
        var linhas = _parser.Parse(arquivo);
        var ativos = (await _ativoRepository.GetAllAsync())
            .ToDictionary(a => a.Ticker, StringComparer.OrdinalIgnoreCase);

        var preview = new List<LinhaImportacaoProventoB3Dto>();

        foreach (var linha in linhas)
        {
            var ativoCadastrado = ativos.TryGetValue(linha.Ticker, out var ativo);
            var jaImportado = await _proventoRepository.ExistsSimilarAsync(
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
