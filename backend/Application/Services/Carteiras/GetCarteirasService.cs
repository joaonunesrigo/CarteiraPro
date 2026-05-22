using Application.Services.Carteiras.DTOs;
using Domain.Interfaces;

namespace Application.Services.Carteiras;

public class GetCarteirasService
{
    private readonly ICarteiraRepository _carteiraRepository;

    public GetCarteirasService(ICarteiraRepository carteiraRepository)
    {
        _carteiraRepository = carteiraRepository;
    }

    public async Task<IReadOnlyList<CarteiraDto>> ExecuteAsync()
    {
        var carteiras = await _carteiraRepository.GetAllAsync();

        return carteiras
            .Select(c => new CarteiraDto(c.Id, c.Nome, c.Descricao, c.Moeda, c.Padrao, c.DataCadastro))
            .ToList();
    }
}
