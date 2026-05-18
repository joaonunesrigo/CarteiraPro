using Domain.Entities;
using Domain.Interfaces;
using Domain.Enums;

namespace Application.Services.Ativos;

public class AddAtivoService
{
    private readonly IAtivoRepository _ativoRepository;

    public AddAtivoService(IAtivoRepository ativoRepository)
    {
        _ativoRepository = ativoRepository;
    }

    public async Task ExecuteAsync(string ticker, string nome, decimal precoMedio, decimal quantidade, TipoAtivo tipo)
    {
        var ativo = new Ativo(ticker, nome, precoMedio, quantidade, tipo);
        await _ativoRepository.AddAsync(ativo);
    }
}