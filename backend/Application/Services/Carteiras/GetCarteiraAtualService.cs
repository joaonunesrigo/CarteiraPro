using Domain.Interfaces;
using CarteiraEntity = Domain.Entities.Carteira;

namespace Application.Services.Carteiras;

public class GetCarteiraAtualService
{
    private readonly ICarteiraRepository _carteiraRepository;

    public GetCarteiraAtualService(ICarteiraRepository carteiraRepository)
    {
        _carteiraRepository = carteiraRepository;
    }

    public async Task<CarteiraEntity> ExecuteAsync(Guid? carteiraId)
    {
        var carteira = carteiraId.HasValue
            ? await _carteiraRepository.GetByIdAsync(carteiraId.Value)
            : await _carteiraRepository.GetPadraoAsync();

        return carteira ?? throw new InvalidOperationException("Carteira não encontrada.");
    }
}
