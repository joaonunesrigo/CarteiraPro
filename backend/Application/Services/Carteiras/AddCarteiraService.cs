using Domain.Enums;
using Domain.Interfaces;
using CarteiraEntity = Domain.Entities.Carteira;

namespace Application.Services.Carteiras;

public class AddCarteiraService
{
    private readonly ICarteiraRepository _carteiraRepository;
    private readonly ICurrentUser _currentUser;

    public AddCarteiraService(ICarteiraRepository carteiraRepository, ICurrentUser currentUser)
    {
        _carteiraRepository = carteiraRepository;
        _currentUser = currentUser;
    }

    public async Task<Guid> ExecuteAsync(string nome, Moeda moeda, string? descricao = null, bool padrao = false)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("Informe o nome da carteira.");

        if (await _carteiraRepository.NomeExisteAsync(nome))
            throw new ArgumentException("Já existe uma carteira com esse nome.");

        var usuarioId = _currentUser.UsuarioId
            ?? throw new InvalidOperationException("Usuário autenticado não encontrado.");

        var carteirasExistentes = (await _carteiraRepository.GetAllAsync()).ToList();
        var deveSerPadrao = carteirasExistentes.Count == 0 || padrao;

        if (deveSerPadrao && carteirasExistentes.Count > 0)
        {
            foreach (var outra in carteirasExistentes.Where(c => c.Padrao))
            {
                outra.RemoverPadrao();
                await _carteiraRepository.UpdateAsync(outra);
            }
        }

        var carteira = new CarteiraEntity(usuarioId, nome, moeda, descricao, padrao: deveSerPadrao);
        await _carteiraRepository.AddAsync(carteira);

        return carteira.Id;
    }
}
