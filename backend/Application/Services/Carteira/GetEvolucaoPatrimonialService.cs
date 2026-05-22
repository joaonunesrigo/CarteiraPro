using Application.Services.Carteira.DTOs;
using Application.Services.Carteiras;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Domain.Models;

namespace Application.Services.Carteira;

public class GetEvolucaoPatrimonialService
{
    private const int LimiteMesesMaximo = 120;

    private readonly IAtivoRepository _ativoRepository;
    private readonly IOperacaoRepository _operacaoRepository;
    private readonly IHistoricoCotacoesCache _historicoCache;
    private readonly ICotacoesCache _cotacoesCache;
    private readonly GetCarteiraAtualService _getCarteiraAtual;

    public GetEvolucaoPatrimonialService(
        IAtivoRepository ativoRepository,
        IOperacaoRepository operacaoRepository,
        IHistoricoCotacoesCache historicoCache,
        ICotacoesCache cotacoesCache,
        GetCarteiraAtualService getCarteiraAtual)
    {
        _ativoRepository = ativoRepository;
        _operacaoRepository = operacaoRepository;
        _historicoCache = historicoCache;
        _cotacoesCache = cotacoesCache;
        _getCarteiraAtual = getCarteiraAtual;
    }

    public async Task<EvolucaoPatrimonialDto> ExecuteAsync(int meses, Guid? carteiraId = null)
    {
        var carteira = await _getCarteiraAtual.ExecuteAsync(carteiraId);
        var ativos = (await _ativoRepository.GetAllAsync(carteira.Id)).ToList();
        if (ativos.Count == 0)
            return new EvolucaoPatrimonialDto();

        var operacoes = (await _operacaoRepository.GetAllAsync(carteira.Id))
            .OrderBy(o => o.Data)
            .ThenBy(o => o.DataCadastro)
            .ToList();
        if (operacoes.Count == 0)
            return new EvolucaoPatrimonialDto();

        var hoje = DateTime.UtcNow.Date;
        var inicioMesAtual = new DateTime(hoje.Year, hoje.Month, 1, 0, 0, 0, DateTimeKind.Utc);

        var primeiraOperacao = operacoes.First().Data;
        var inicioPrimeiraOperacao = new DateTime(primeiraOperacao.Year, primeiraOperacao.Month, 1, 0, 0, 0, DateTimeKind.Utc);

        var inicio = meses <= 0
            ? inicioPrimeiraOperacao
            : MaxData(inicioPrimeiraOperacao, AdicionarMeses(inicioMesAtual, -(meses - 1)));

        var totalMesesJanela = ContarMeses(inicio, inicioMesAtual);
        var totalMesesHistorico = ContarMeses(inicioPrimeiraOperacao, inicioMesAtual);
        var mesesParaBrapi = Math.Clamp(totalMesesHistorico, 1, LimiteMesesMaximo);

        var tickers = ativos.Select(a => a.Ticker).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
        var historicos = await _historicoCache.ObterHistoricoMensalAsync(tickers, mesesParaBrapi);
        var cotacoesAtuais = await _cotacoesCache.ObterQuotesAsync(tickers);

        var operacoesPorAtivo = operacoes.GroupBy(o => o.AtivoId).ToDictionary(g => g.Key, g => g.ToList());
        var pontos = new List<EvolucaoPatrimonialPontoDto>(totalMesesJanela + 1);

        for (var i = 0; i <= totalMesesJanela; i++)
        {
            var refMes = AdicionarMeses(inicio, i);
            var fimMes = DataDeReferencia(refMes, hoje);

            decimal patrimonio = 0;
            decimal valorInvestido = 0;

            foreach (var ativo in ativos)
            {
                if (!operacoesPorAtivo.TryGetValue(ativo.Id, out var operacoesAtivo))
                    continue;

                var (quantidade, custoMedio) = CalcularPosicaoAteData(operacoesAtivo, fimMes);
                if (quantidade <= 0)
                    continue;

                valorInvestido += quantidade * custoMedio;

                decimal? cotacao = null;
                var ehMesCorrente = refMes >= inicioMesAtual;
                if (ehMesCorrente && cotacoesAtuais.TryGetValue(ativo.Ticker, out var quoteAtual))
                    cotacao = quoteAtual.Cotacao;
                cotacao ??= ObterCotacaoNoMes(historicos, ativo.Ticker, refMes.Year, refMes.Month);
                cotacao ??= custoMedio;

                patrimonio += quantidade * cotacao.Value;
            }

            pontos.Add(new EvolucaoPatrimonialPontoDto
            {
                Data = refMes,
                Patrimonio = Math.Round(patrimonio, 2, MidpointRounding.AwayFromZero),
                ValorInvestido = Math.Round(valorInvestido, 2, MidpointRounding.AwayFromZero),
            });
        }

        var tickersComHistorico = historicos.Keys.ToHashSet(StringComparer.OrdinalIgnoreCase);
        var semHistorico = tickers.Where(t => !tickersComHistorico.Contains(t)).ToList();

        return new EvolucaoPatrimonialDto
        {
            Pontos = pontos,
            TickersSemHistorico = semHistorico,
        };
    }

    private static (decimal Quantidade, decimal CustoMedio) CalcularPosicaoAteData(IEnumerable<Operacao> operacoes, DateTime ate)
    {
        decimal quantidade = 0;
        decimal precoMedio = 0;

        foreach (var operacao in operacoes.Where(o => o.Data <= ate))
        {
            if (operacao.Tipo == TipoOperacao.Compra)
            {
                var custoAtual = quantidade * precoMedio;
                var custoCompra = operacao.ValorBruto + operacao.Taxas;
                quantidade += operacao.Quantidade;
                precoMedio = quantidade > 0 ? (custoAtual + custoCompra) / quantidade : 0;
                continue;
            }

            if (operacao.Quantidade > quantidade)
                continue;

            quantidade -= operacao.Quantidade;
            if (quantidade == 0)
                precoMedio = 0;
        }

        return (quantidade, precoMedio);
    }

    private static decimal? ObterCotacaoNoMes(
        IReadOnlyDictionary<string, IReadOnlyList<HistoricoCotacaoMensal>> historicos,
        string ticker,
        int ano,
        int mes)
    {
        if (!historicos.TryGetValue(ticker, out var serie) || serie.Count == 0)
            return null;

        var exato = serie.FirstOrDefault(h => h.Ano == ano && h.Mes == mes);
        if (exato is not null)
            return exato.Fechamento;

        var anterior = serie.Where(h => h.Ano < ano || (h.Ano == ano && h.Mes < mes))
            .OrderByDescending(h => h.Ano).ThenByDescending(h => h.Mes)
            .FirstOrDefault();

        return anterior?.Fechamento;
    }

    private static DateTime AdicionarMeses(DateTime data, int meses)
    {
        var resultado = data.AddMonths(meses);
        return DateTime.SpecifyKind(resultado, DateTimeKind.Utc);
    }

    private static int ContarMeses(DateTime inicio, DateTime fim)
    {
        var meses = ((fim.Year - inicio.Year) * 12) + (fim.Month - inicio.Month);
        return Math.Max(0, meses);
    }

    private static DateTime MaxData(DateTime a, DateTime b) => a > b ? a : b;

    private static DateTime DataDeReferencia(DateTime refMes, DateTime hoje)
    {
        var ultimoDia = DateTime.SpecifyKind(
            new DateTime(refMes.Year, refMes.Month, DateTime.DaysInMonth(refMes.Year, refMes.Month)),
            DateTimeKind.Utc);

        return ultimoDia > hoje ? hoje : ultimoDia;
    }
}
