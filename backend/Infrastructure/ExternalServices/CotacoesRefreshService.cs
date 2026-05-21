using Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Infrastructure.ExternalServices;

public class CotacoesRefreshService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<CotacoesRefreshService> _logger;
    private static readonly TimeSpan Intervalo = TimeSpan.FromMinutes(30);

    public CotacoesRefreshService(
        IServiceScopeFactory scopeFactory,
        ILogger<CotacoesRefreshService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await AtualizarCotacoesAsync(stoppingToken);

        using var timer = new PeriodicTimer(Intervalo);
        while (await timer.WaitForNextTickAsync(stoppingToken))
            await AtualizarCotacoesAsync(stoppingToken);
    }

    private async Task AtualizarCotacoesAsync(CancellationToken cancellationToken)
    {
        try
        {
            using var scope = _scopeFactory.CreateScope();
            var ativoRepository = scope.ServiceProvider.GetRequiredService<IAtivoRepository>();
            var cotacoesCache = scope.ServiceProvider.GetRequiredService<ICotacoesCache>();

            var tickers = (await ativoRepository.GetAllAsync())
                .Select(ativo => ativo.Ticker)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (tickers.Count == 0)
                return;

            await cotacoesCache.AtualizarQuotesAsync(tickers);
            _logger.LogInformation("Cotacoes atualizadas em background para {Quantidade} tickers.", tickers.Count);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Falha ao atualizar cotacoes em background.");
        }
    }
}
