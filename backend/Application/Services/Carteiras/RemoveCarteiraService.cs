using Domain.Interfaces;

namespace Application.Services.Carteiras;

public class RemoveCarteiraService
{
    private readonly ICarteiraRepository _carteiraRepository;

    public RemoveCarteiraService(ICarteiraRepository carteiraRepository)
    {
        _carteiraRepository = carteiraRepository;
    }

    public async Task ExecuteAsync(Guid id)
    {
        var carteiras = (await _carteiraRepository.GetAllAsync()).ToList();
        var carteira = carteiras.FirstOrDefault(c => c.Id == id)
            ?? throw new InvalidOperationException("Carteira não encontrada.");

        if (carteiras.Count == 1)
            throw new ArgumentException("Não é possível excluir a única carteira.");

        if (carteira.Padrao)
            throw new ArgumentException("Defina outra carteira como padrão antes de excluir esta.");

        await _carteiraRepository.DeleteAsync(carteira);
    }
}
