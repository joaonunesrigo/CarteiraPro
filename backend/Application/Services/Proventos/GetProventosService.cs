using Application.Dtos;
using Application.Services.Carteiras;
using Domain.Interfaces;

namespace Application.Services.Proventos;

public class GetProventosService
{
    private readonly IProventoRepository _proventoRepository;
    private readonly GetCarteiraAtualService _getCarteiraAtual;

    public GetProventosService(IProventoRepository proventoRepository, GetCarteiraAtualService getCarteiraAtual)
    {
        _proventoRepository = proventoRepository;
        _getCarteiraAtual = getCarteiraAtual;
    }

    public async Task<IReadOnlyList<ProventoDto>> ExecuteAsync(Guid? carteiraId = null, Guid? ativoId = null, DateTime? dataInicio = null, DateTime? dataFim = null)
    {
        var carteira = await _getCarteiraAtual.ExecuteAsync(carteiraId);
        var proventos = await _proventoRepository.GetAllAsync(carteira.Id, ativoId, dataInicio, dataFim);

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
