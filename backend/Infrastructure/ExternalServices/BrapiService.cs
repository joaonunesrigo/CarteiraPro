using System.Net;
using System.Text.Json;
using Domain.Interfaces;
using Domain.Models;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.ExternalServices;

public class BrapiService : IBrapiService
{
    private readonly HttpClient _httpClient;
    private readonly string _token;
    private const string BaseUrl = "https://brapi.dev/api";

    public BrapiService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _token = configuration["Brapi:Token"]!;
    }

    public async Task<IReadOnlyDictionary<string, BrapiQuote>> ObterQuotesAsync(
        IEnumerable<string> tickers)
    {
        var lista = tickers
            .Select(t => t.Trim().ToUpper())
            .Where(t => !string.IsNullOrWhiteSpace(t))
            .Distinct()
            .ToList();

        if (lista.Count == 0)
            return new Dictionary<string, BrapiQuote>();

        var mapa = new Dictionary<string, BrapiQuote>(StringComparer.OrdinalIgnoreCase);

        if (lista.Count > 1)
        {
            var lote = await ObterQuotesEmLoteAsync(lista);
            foreach (var par in lote)
                mapa[par.Key] = par.Value;
        }

        var faltantes = lista.Where(t => !mapa.ContainsKey(t)).ToList();
        if (faltantes.Count == 0)
            return mapa;

        var individuais = await Task.WhenAll(
            faltantes.Select(async ticker =>
            {
                var quote = await ObterQuoteIndividualAsync(ticker);
                return (ticker, quote);
            }));

        foreach (var (ticker, quote) in individuais)
        {
            if (quote is not null)
                mapa[ticker] = quote;
        }

        return mapa;
    }

    public async Task<BrapiQuote?> ObterQuoteAsync(string ticker)
    {
        return await ObterQuoteIndividualAsync(ticker.Trim().ToUpper());
    }

    public async Task<decimal> GetCotacaoAsync(string ticker)
    {
        var quote = await ObterQuoteAsync(ticker)
            ?? throw new InvalidOperationException($"Cotação não encontrada para {ticker}");

        return quote.Cotacao;
    }

    public async Task<string> GetNomeAtivoAsync(string ticker)
    {
        var quote = await ObterQuoteAsync(ticker)
            ?? throw new InvalidOperationException($"Ativo não encontrado para {ticker}");

        return quote.Nome;
    }

    private async Task<BrapiQuote?> ObterQuoteIndividualAsync(string ticker)
    {
        var response = await _httpClient.GetAsync(
            $"{BaseUrl}/quote/{ticker}?token={_token}");

        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;

        if (!response.IsSuccessStatusCode)
            return null;

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);

        if (!doc.RootElement.TryGetProperty("results", out var results)
            || results.GetArrayLength() == 0)
            return null;

        return LerQuoteDoResultado(results[0], ticker);
    }

    private async Task<IReadOnlyDictionary<string, BrapiQuote>> ObterQuotesEmLoteAsync(
        List<string> tickers)
    {
        var tickersParam = string.Join(',', tickers);
        var response = await _httpClient.GetAsync(
            $"{BaseUrl}/quote/{tickersParam}?token={_token}");

        if (!response.IsSuccessStatusCode)
            return new Dictionary<string, BrapiQuote>();

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);

        if (!doc.RootElement.TryGetProperty("results", out var results))
            return new Dictionary<string, BrapiQuote>();

        var mapa = new Dictionary<string, BrapiQuote>(StringComparer.OrdinalIgnoreCase);

        foreach (var item in results.EnumerateArray())
        {
            var symbol = ObterSymbol(item);
            if (string.IsNullOrEmpty(symbol)) continue;

            var quote = LerQuoteDoResultado(item, symbol);
            if (quote is not null)
                mapa[symbol] = quote;
        }

        return mapa;
    }

    private static string? ObterSymbol(JsonElement item)
    {
        if (item.TryGetProperty("symbol", out var symbol))
            return symbol.GetString()?.ToUpper();

        if (item.TryGetProperty("stock", out var stock))
            return stock.GetString()?.ToUpper();

        return null;
    }

    private static BrapiQuote? LerQuoteDoResultado(JsonElement item, string tickerFallback)
    {
        if (!item.TryGetProperty("regularMarketPrice", out var precoElement))
            return null;

        var ticker = ObterSymbol(item) ?? tickerFallback;

        var nome = item.TryGetProperty("shortName", out var nomeElement)
            ? nomeElement.GetString() ?? ticker
            : ticker;

        return new BrapiQuote
        {
            Nome = nome,
            Cotacao = precoElement.GetDecimal(),
        };
    }
}
