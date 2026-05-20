using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services.Ativos;

public class GetAtivosService
{
    private readonly IAtivoRepository _ativoRepository;

    public GetAtivosService(IAtivoRepository ativoRepository)
    {
        _ativoRepository = ativoRepository;
    }

    public async Task<IEnumerable<Ativo>> ExecuteAsync()
    {
        return await _ativoRepository.GetAllAsync();
    }
}