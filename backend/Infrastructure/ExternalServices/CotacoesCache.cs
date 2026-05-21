using System.Collections.Concurrent;
using Domain.Interfaces;
using Domain.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Infrastructure.ExternalServices;

public class CotacoesCache : ICotacoesCache
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<CotacoesCache> _logger;
    private readonly TimeSpan _tempoExpiracao;
    private readonly ConcurrentDictionary<string, CotacaoCacheItem> _cache = new(StringComparer.OrdinalIgnoreCase);
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    public CotacoesCache(
        IServiceScopeFactory scopeFactory,
        IConfiguration configuration,
        ILogger<CotacoesCache> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
        var minutos = int.TryParse(configuration["Brapi:CacheMinutos"], out var configurado)
            ? configurado
            : 30;
        _tempoExpiracao = TimeSpan.FromMinutes(Math.Max(1, minutos));
    }

    public async Task<IReadOnlyDictionary<string, BrapiQuote>> ObterQuotesAsync(IEnumerable<string> tickers)
    {
        var lista = NormalizarTickers(tickers);
        if (lista.Count == 0)
            return new Dictionary<string, BrapiQuote>();

        var expiradosOuAusentes = lista
            .Where(ticker => !_cache.TryGetValue(ticker, out var item) || item.EstaExpirado(_tempoExpiracao))
            .ToList();

        if (expiradosOuAusentes.Count > 0)
            await AtualizarQuotesAsync(expiradosOuAusentes);

        return lista
            .Where(ticker => _cache.ContainsKey(ticker))
            .ToDictionary(
                ticker => ticker,
                ticker => _cache[ticker].Quote,
                StringComparer.OrdinalIgnoreCase);
    }

    public async Task AtualizarQuotesAsync(IEnumerable<string> tickers)
    {
        var lista = NormalizarTickers(tickers);
        if (lista.Count == 0)
            return;

        await _semaphore.WaitAsync();
        try
        {
            var aindaNecessarios = lista
                .Where(ticker => !_cache.TryGetValue(ticker, out var item) || item.EstaExpirado(_tempoExpiracao))
                .ToList();

            if (aindaNecessarios.Count == 0)
                return;

            using var scope = _scopeFactory.CreateScope();
            var brapiService = scope.ServiceProvider.GetRequiredService<IBrapiService>();
            var cotacoes = await brapiService.ObterQuotesAsync(aindaNecessarios);
            var agora = DateTime.UtcNow;

            foreach (var (ticker, quote) in cotacoes)
                _cache[ticker] = new CotacaoCacheItem(quote, agora);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Nao foi possivel atualizar cotacoes da Brapi. Usando cache existente quando disponivel.");
        }
        finally
        {
            _semaphore.Release();
        }
    }

    private static List<string> NormalizarTickers(IEnumerable<string> tickers)
    {
        return tickers
            .Select(ticker => ticker.Trim().ToUpperInvariant())
            .Where(ticker => !string.IsNullOrWhiteSpace(ticker))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private sealed record CotacaoCacheItem(BrapiQuote Quote, DateTime AtualizadoEm)
    {
        public bool EstaExpirado(TimeSpan tempoExpiracao)
        {
            return DateTime.UtcNow - AtualizadoEm >= tempoExpiracao;
        }
    }
}
