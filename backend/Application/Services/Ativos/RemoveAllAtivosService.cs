using Application.Services.Carteiras;
using Domain.Interfaces;

namespace Application.Services.Ativos;

public class RemoveAllAtivosService
{
    private readonly IAtivoRepository _ativoRepository;
    private readonly GetCarteiraAtualService _getCarteiraAtual;

    public RemoveAllAtivosService(IAtivoRepository ativoRepository, GetCarteiraAtualService getCarteiraAtual)
    {
        _ativoRepository = ativoRepository;
        _getCarteiraAtual = getCarteiraAtual;
    }

    public async Task<int> ExecuteAsync(Guid? carteiraId = null)
    {
        var carteira = await _getCarteiraAtual.ExecuteAsync(carteiraId);
        return await _ativoRepository.DeleteAllAsync(carteira.Id);
    }
}
