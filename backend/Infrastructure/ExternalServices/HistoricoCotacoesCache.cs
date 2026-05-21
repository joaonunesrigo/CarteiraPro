using System.Collections.Concurrent;
using Domain.Interfaces;
using Domain.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Infrastructure.ExternalServices;

public class HistoricoCotacoesCache : IHistoricoCotacoesCache
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<HistoricoCotacoesCache> _logger;
    private readonly TimeSpan _tempoExpiracao;
    private readonly ConcurrentDictionary<string, HistoricoCacheItem> _cache = new(StringComparer.OrdinalIgnoreCase);
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    public HistoricoCotacoesCache(
        IServiceScopeFactory scopeFactory,
        IConfiguration configuration,
        ILogger<HistoricoCotacoesCache> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
        var horas = int.TryParse(configuration["Brapi:HistoricoCacheHoras"], out var configurado) ? configurado : 24;
        _tempoExpiracao = TimeSpan.FromHours(Math.Max(1, horas));
    }

    public async Task<IReadOnlyDictionary<string, IReadOnlyList<HistoricoCotacaoMensal>>> ObterHistoricoMensalAsync(
        IEnumerable<string> tickers,
        int meses)
    {
        var lista = tickers
            .Select(t => t.Trim().ToUpperInvariant())
            .Where(t => !string.IsNullOrWhiteSpace(t))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (lista.Count == 0)
            return new Dictionary<string, IReadOnlyList<HistoricoCotacaoMensal>>();

        var faltantes = lista
            .Where(ticker => !_cache.TryGetValue(ChaveCache(ticker, meses), out var item)
                || item.EstaExpirado(_tempoExpiracao))
            .ToList();

        if (faltantes.Count > 0)
            await AtualizarAsync(faltantes, meses);

        return lista
            .Where(ticker => _cache.ContainsKey(ChaveCache(ticker, meses)))
            .ToDictionary(
                ticker => ticker,
                ticker => _cache[ChaveCache(ticker, meses)].Historico,
                StringComparer.OrdinalIgnoreCase);
    }

    private async Task AtualizarAsync(List<string> tickers, int meses)
    {
        await _semaphore.WaitAsync();
        try
        {
            using var scope = _scopeFactory.CreateScope();
            var brapiService = scope.ServiceProvider.GetRequiredService<IBrapiService>();
            var agora = DateTime.UtcNow;

            foreach (var ticker in tickers)
            {
                var chave = ChaveCache(ticker, meses);
                if (_cache.TryGetValue(chave, out var existente) && !existente.EstaExpirado(_tempoExpiracao))
                    continue;

                try
                {
                    var historico = await brapiService.ObterHistoricoMensalAsync(ticker, meses);
                    _cache[chave] = new HistoricoCacheItem(historico, agora);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Falha ao obter historico mensal para {Ticker}.", ticker);
                }
            }
        }
        finally
        {
            _semaphore.Release();
        }
    }

    private static string ChaveCache(string ticker, int meses) => $"{ticker}|{meses}";

    private sealed record HistoricoCacheItem(IReadOnlyList<HistoricoCotacaoMensal> Historico, DateTime AtualizadoEm)
    {
        public bool EstaExpirado(TimeSpan tempoExpiracao) => DateTime.UtcNow - AtualizadoEm >= tempoExpiracao;
    }
}
