using Domain.Interfaces;

namespace Application.Services.Ativos;

public class RemoveAllAtivosService
{
    private readonly IAtivoRepository _ativoRepository;

    public RemoveAllAtivosService(IAtivoRepository ativoRepository)
    {
        _ativoRepository = ativoRepository;
    }

    public async Task<int> ExecuteAsync()
    {
        return await _ativoRepository.DeleteAllAsync();
    }
}
