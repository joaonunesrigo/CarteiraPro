using Domain.Entities;
using Domain.Interfaces;
using Domain.Enums;

namespace Application.UseCases.Ativos;

public class AddAtivoUseCase
{
    private readonly IAtivoRepository _ativoRepository;

    public AddAtivoUseCase(IAtivoRepository ativoRepository)
    {
        _ativoRepository = ativoRepository;
    }

    public async Task ExecuteAsync(string ticker, string nome, decimal precoMedio, decimal quantidade, TipoAtivo tipo)
    {
        var ativo = new Ativo(ticker, nome, precoMedio, quantidade, tipo);
        await _ativoRepository.AddAsync(ativo);
    }
}