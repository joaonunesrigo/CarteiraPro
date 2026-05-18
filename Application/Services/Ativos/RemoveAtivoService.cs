using Domain.Interfaces;

namespace Application.Services.Ativos;

public class RemoveAtivoService
{
    private readonly IAtivoRepository _ativoRepository;

    public RemoveAtivoService(IAtivoRepository ativoRepository)
    {
        _ativoRepository = ativoRepository;
    }

    public async Task ExecuteAsync(Guid id)
    {
        await _ativoRepository.DeleteAsync(id);
    }
}