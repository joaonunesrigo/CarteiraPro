using Application.Dtos;
using Domain.Interfaces;

namespace Application.Services.Proventos;

public class GetProventosService
{
    private readonly IProventoRepository _proventoRepository;

    public GetProventosService(IProventoRepository proventoRepository)
    {
        _proventoRepository = proventoRepository;
    }

    public async Task<IReadOnlyList<ProventoDto>> ExecuteAsync(Guid? ativoId = null, DateTime? dataInicio = null, DateTime? dataFim = null)
    {
        var proventos = await _proventoRepository.GetAllAsync(ativoId, dataInicio, dataFim);

        return proventos
            .Select(p => new ProventoDto(
                p.Id,
                p.AtivoId,
                p.Ticker,
                p.ValorPorCota,
                p.Quantidade,
                p.ValorTotal,
                p.DataPagamento,
                p.Tipo))
            .ToList();
    }
}
