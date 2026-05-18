using Domain.Entities;
using Domain.Interfaces;
using Domain.Enums;

namespace Application.Services.Ativos;

public class AddAtivoService
{
    private readonly IBrapiService _brapiService;

    private readonly IAtivoRepository _ativoRepository;

    public AddAtivoService(IAtivoRepository ativoRepository, IBrapiService brapiService)
    {
        _ativoRepository = ativoRepository;
        _brapiService = brapiService;
    }

    public async Task ExecuteAsync(string ticker, decimal precoMedio, decimal quantidade, TipoAtivo tipo)
    {
        var nome = await _brapiService.GetNomeAtivoAsync(ticker);
        var ativo = new Ativo(ticker, nome, precoMedio, quantidade, tipo);
        await _ativoRepository.AddAsync(ativo);
    }
}