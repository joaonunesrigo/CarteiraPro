using Domain.Interfaces;

namespace Application.Services.Ativos;

public class GetCotacaoAtivoService
{
    private readonly IBrapiService _brapiService;

    public GetCotacaoAtivoService(IBrapiService brapiService)
    {
        _brapiService = brapiService;
    }

    public async Task<decimal> ExecuteAsync(string ticker)
    {
        return await _brapiService.GetCotacaoAsync(ticker);
    }
}