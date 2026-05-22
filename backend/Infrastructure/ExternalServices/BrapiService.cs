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

    public async Task<IReadOnlyList<HistoricoCotacaoMensal>> ObterHistoricoMensalAsync(string ticker, int meses)
    {
        var tickerNormalizado = ticker.Trim().ToUpperInvariant();
        var range = MapearRangeBrapi(meses);

        var response = await _httpClient.GetAsync(
            $"{BaseUrl}/quote/{tickerNormalizado}?range={range}&interval=1mo&token={_token}");

        if (!response.IsSuccessStatusCode)
            return [];

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);

        if (!doc.RootElement.TryGetProperty("results", out var results) || results.GetArrayLength() == 0)
            return [];

        var primeiroResultado = results[0];
        if (!primeiroResultado.TryGetProperty("historicalDataPrice", out var historico)
            || historico.ValueKind != JsonValueKind.Array)
            return [];

        var fechamentosPorMes = new Dictionary<(int Ano, int Mes), decimal>();

        foreach (var item in historico.EnumerateArray())
        {
            var data = ObterDataHistorico(item);
            var fechamento = ObterFechamentoHistorico(item);
            if (data is null || fechamento is null)
                continue;

            var chave = (data.Value.Year, data.Value.Month);
            fechamentosPorMes[chave] = fechamento.Value;
        }

        return fechamentosPorMes
            .Select(par => new HistoricoCotacaoMensal(par.Key.Ano, par.Key.Mes, par.Value))
            .OrderBy(item => item.Ano).ThenBy(item => item.Mes)
            .ToList();
    }

    private static string MapearRangeBrapi(int meses)
    {
        return meses switch
        {
            <= 1 => "1mo",
            <= 3 => "3mo",
            <= 6 => "6mo",
            <= 12 => "1y",
            <= 24 => "2y",
            <= 60 => "5y",
            <= 120 => "10y",
            _ => "max",
        };
    }

    private static DateTime? ObterDataHistorico(JsonElement item)
    {
        if (!item.TryGetProperty("date", out var dataElement))
            return null;

        if (dataElement.ValueKind == JsonValueKind.Number && dataElement.TryGetInt64(out var timestamp))
            return DateTimeOffset.FromUnixTimeSeconds(timestamp).UtcDateTime;

        if (dataElement.ValueKind == JsonValueKind.String && DateTime.TryParse(dataElement.GetString(), out var data))
            return DateTime.SpecifyKind(data, DateTimeKind.Utc);

        return null;
    }

    private static decimal? ObterFechamentoHistorico(JsonElement item)
    {
        if (item.TryGetProperty("adjustedClose", out var ajustado)
            && ajustado.ValueKind == JsonValueKind.Number)
            return ajustado.GetDecimal();

        if (item.TryGetProperty("close", out var fechamento)
            && fechamento.ValueKind == JsonValueKind.Number)
            return fechamento.GetDecimal();

        return null;
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
            RegularMarketTime = ObterRegularMarketTime(item),
        };
    }

    public async Task<string?> ObterSetorAsync(string ticker)
    {
        var tickerNormalizado = ticker.Trim().ToUpperInvariant();
        if (string.IsNullOrWhiteSpace(tickerNormalizado))
            return null;

        var response = await _httpClient.GetAsync(
            $"{BaseUrl}/quote/{tickerNormalizado}?modules=summaryProfile&token={_token}");

        if (!response.IsSuccessStatusCode)
            return null;

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);

        if (!doc.RootElement.TryGetProperty("results", out var results)
            || results.ValueKind != JsonValueKind.Array
            || results.GetArrayLength() == 0)
            return null;

        if (!results[0].TryGetProperty("summaryProfile", out var profile)
            || profile.ValueKind != JsonValueKind.Object)
            return null;

        var setor = LerCampoTexto(profile, "sector") ?? LerCampoTexto(profile, "industry");
        return setor;
    }

    private static string? LerCampoTexto(JsonElement obj, string propriedade)
    {
        if (!obj.TryGetProperty(propriedade, out var element)
            || element.ValueKind != JsonValueKind.String)
            return null;

        var valor = element.GetString();
        return string.IsNullOrWhiteSpace(valor) ? null : valor.Trim();
    }

    private static DateTime? ObterRegularMarketTime(JsonElement item)
    {
        if (!item.TryGetProperty("regularMarketTime", out var marketTime))
            return null;

        if (marketTime.ValueKind == JsonValueKind.String
            && DateTime.TryParse(marketTime.GetString(), out var data))
            return DateTime.SpecifyKind(data, DateTimeKind.Utc);

        if (marketTime.ValueKind == JsonValueKind.Number
            && marketTime.TryGetInt64(out var timestamp))
            return DateTimeOffset.FromUnixTimeSeconds(timestamp).UtcDateTime;

        return null;
    }
}
