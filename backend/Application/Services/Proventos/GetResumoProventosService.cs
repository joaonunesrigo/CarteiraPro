using Application.Dtos;
using Domain.Interfaces;

namespace Application.Services.Proventos;

public class GetResumoProventosService
{
    private readonly GetProventosService _getProventos;

    public GetResumoProventosService(GetProventosService getProventos)
    {
        _getProventos = getProventos;
    }

    public async Task<ResumoProventosDto> ExecuteAsync(Guid? ativoId = null, DateTime? dataInicio = null, DateTime? dataFim = null)
    {
        var proventos = await _getProventos.ExecuteAsync(ativoId, dataInicio, dataFim);
        var totalRecebido = proventos.Sum(p => p.ValorTotal);

        var porMes = proventos
            .GroupBy(p => p.DataPagamento.ToString("yyyy-MM"))
            .OrderBy(g => g.Key)
            .Select(g => new ResumoProventosMesDto(g.Key, g.Sum(p => p.ValorTotal)))
            .ToList();

        var porAtivo = proventos
            .GroupBy(p => p.Ticker)
            .OrderByDescending(g => g.Sum(p => p.ValorTotal))
            .Select(g => new ResumoProventosAtivoDto(g.Key, g.Sum(p => p.ValorTotal)))
            .ToList();

        return new ResumoProventosDto(totalRecebido, porMes, porAtivo);
    }
}
