using Application.Catalogs;
using Application.Services.Ativos.DTOs;
using Application.Services.Carteiras;
using Application.Services.Operacoes;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;

namespace Application.Services.Carteira;

public class GetRentabilidadeAtivosService
{
    private const int ConcorrenciaEnriquecimentoSetor = 4;

    private readonly IAtivoRepository _ativoRepository;
    private readonly ICotacoesCache _cotacoesCache;
    private readonly IBrapiService _brapiService;
    private readonly CalcularPosicaoAtivoService _calcularPosicao;
    private readonly GetCarteiraAtualService _getCarteiraAtual;

    public GetRentabilidadeAtivosService(
        IAtivoRepository ativoRepository,
        ICotacoesCache cotacoesCache,
        IBrapiService brapiService,
        CalcularPosicaoAtivoService calcularPosicao,
        GetCarteiraAtualService getCarteiraAtual)
    {
        _ativoRepository = ativoRepository;
        _cotacoesCache = cotacoesCache;
        _brapiService = brapiService;
        _calcularPosicao = calcularPosicao;
        _getCarteiraAtual = getCarteiraAtual;
    }

    public async Task<IEnumerable<RentabilidadeDto>> ExecuteAsync(Guid? carteiraId = null)
    {
        var carteira = await _getCarteiraAtual.ExecuteAsync(carteiraId);
        var ativos = (await _ativoRepository.GetAllAsync(carteira.Id)).ToList();
        if (ativos.Count == 0)
            return [];

        var cotacoes = await _cotacoesCache.ObterQuotesAsync(ativos.Select(a => a.Ticker));
        var posicoes = await _calcularPosicao.ExecuteAsync(ativos);

        await EnriquecerSetoresAsync(ativos);

        var resultado = new List<RentabilidadeDto>(ativos.Count);

        foreach (var ativo in ativos)
        {
            var posicao = posicoes[ativo.Id];
            if (posicao.Quantidade <= 0)
                continue;

            var cotacao = cotacoes.TryGetValue(ativo.Ticker, out var quote)
                ? quote.Cotacao
                : posicao.PrecoMedio;

            var valorInvestido = posicao.ValorInvestido;
            var valorAtual = cotacao * posicao.Quantidade;
            var rentabilidadeReais = valorAtual - valorInvestido;
            var rentabilidadePercent = posicao.PrecoMedio > 0
                ? (cotacao - posicao.PrecoMedio) / posicao.PrecoMedio * 100
                : 0;

            resultado.Add(new RentabilidadeDto
            {
                Id = ativo.Id,
                Ticker = ativo.Ticker,
                Tipo = ativo.Tipo,
                Setor = ResolverSetor(ativo),
                PrecoMedio = posicao.PrecoMedio,
                CotacaoAtual = cotacao,
                Quantidade = posicao.Quantidade,
                ValorInvestido = valorInvestido,
                ValorAtual = valorAtual,
                RentabilidadeReais = rentabilidadeReais,
                RentabilidadePercent = Math.Round(rentabilidadePercent, 2),
                CotacaoAtualizadaEm = quote?.RegularMarketTime,
            });
        }

        return resultado;
    }

    private async Task EnriquecerSetoresAsync(IReadOnlyList<Ativo> ativos)
    {
        var reclassificacoesCatalogoFii = ativos
            .Where(a => a.Tipo == TipoAtivo.FII)
            .Select(a => (Ativo: a, Segmento: SegmentosFiisCatalog.TryObter(a.Ticker)))
            .Where(par => !string.IsNullOrWhiteSpace(par.Segmento)
                && !string.Equals(par.Ativo.Setor, par.Segmento, StringComparison.Ordinal))
            .ToList();

        foreach (var (ativo, segmento) in reclassificacoesCatalogoFii)
        {
            ativo.DefinirSetor(segmento);
            await _ativoRepository.UpdateAsync(ativo);
        }

        var pendentesBrapi = ativos
            .Where(a => string.IsNullOrWhiteSpace(a.Setor)
                && (a.Tipo != TipoAtivo.FII || SegmentosFiisCatalog.TryObter(a.Ticker) is null))
            .ToList();

        if (pendentesBrapi.Count > 0)
        {
            using var semaforo = new SemaphoreSlim(ConcorrenciaEnriquecimentoSetor);

            var consultas = pendentesBrapi.Select(async ativo =>
            {
                await semaforo.WaitAsync();
                try
                {
                    var setor = await _brapiService.ObterSetorAsync(ativo.Ticker);
                    return (ativo, setor);
                }
                finally
                {
                    semaforo.Release();
                }
            });

            var resultados = await Task.WhenAll(consultas);

            foreach (var (ativo, setor) in resultados)
            {
                if (string.IsNullOrWhiteSpace(setor)) continue;
                ativo.DefinirSetor(setor);
                await _ativoRepository.UpdateAsync(ativo);
            }
        }

        var fallbacksAcoes = ativos
            .Where(a => a.Tipo == TipoAtivo.AcaoBR && string.IsNullOrWhiteSpace(a.Setor))
            .Select(a => (Ativo: a, Setor: SetoresAcoesCatalog.TryObter(a.Ticker)))
            .Where(par => !string.IsNullOrWhiteSpace(par.Setor))
            .ToList();

        foreach (var (ativo, setor) in fallbacksAcoes)
        {
            ativo.DefinirSetor(setor);
            await _ativoRepository.UpdateAsync(ativo);
        }
    }

    private static string? ResolverSetor(Ativo ativo)
    {
        if (ativo.Tipo == TipoAtivo.FII)
        {
            var segmento = SegmentosFiisCatalog.TryObter(ativo.Ticker);
            if (!string.IsNullOrWhiteSpace(segmento))
                return segmento;
        }

        if (!string.IsNullOrWhiteSpace(ativo.Setor))
            return ativo.Setor;

        if (ativo.Tipo == TipoAtivo.AcaoBR)
            return SetoresAcoesCatalog.TryObter(ativo.Ticker);

        return null;
    }
}
