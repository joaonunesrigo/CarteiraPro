using Domain.Entities;
using Domain.Interfaces;

namespace Application.UseCases.Ativos;

public class GetAtivosUseCase
{
    private readonly IAtivoRepository _ativoRepository;

    public GetAtivosUseCase(IAtivoRepository ativoRepository)
    {
        _ativoRepository = ativoRepository;
    }

    public async Task<IEnumerable<Ativo>> ExecuteAsync()
    {
        return await _ativoRepository.GetAllAsync();
    }
}