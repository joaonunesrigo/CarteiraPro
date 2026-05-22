using Domain.Enums;
using Domain.Interfaces;

namespace Application.Services.Carteiras;

public class UpdateCarteiraService
{
    private readonly ICarteiraRepository _carteiraRepository;

    public UpdateCarteiraService(ICarteiraRepository carteiraRepository)
    {
        _carteiraRepository = carteiraRepository;
    }

    public async Task ExecuteAsync(Guid id, string nome, Moeda moeda, string? descricao = null, bool padrao = false)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("Informe o nome da carteira.");

        var carteira = await _carteiraRepository.GetByIdAsync(id)
            ?? throw new InvalidOperationException("Carteira não encontrada.");

        if (!string.Equals(carteira.Nome, nome.Trim(), StringComparison.OrdinalIgnoreCase)
            && await _carteiraRepository.NomeExisteAsync(nome, id))
            throw new ArgumentException("Já existe uma carteira com esse nome.");

        carteira.Atualizar(nome, moeda, descricao);

        if (padrao && !carteira.Padrao)
        {
            foreach (var outra in await _carteiraRepository.GetAllAsync())
            {
                if (outra.Id == id || !outra.Padrao) continue;
                outra.RemoverPadrao();
                await _carteiraRepository.UpdateAsync(outra);
            }

            carteira.MarcarComoPadrao();
        }

        await _carteiraRepository.UpdateAsync(carteira);
    }
}
