using System.Text.Json;
using Domain.Interfaces;
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

    public async Task<decimal> GetCotacaoAsync(string ticker)
    {
        var response = await _httpClient.GetAsync($"{BaseUrl}/quote/{ticker}?token={_token}");
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var doc = JsonDocument.Parse(json);

        var preco = doc.RootElement
            .GetProperty("results")[0]
            .GetProperty("regularMarketPrice")
            .GetDecimal();

        return preco;
    }
}