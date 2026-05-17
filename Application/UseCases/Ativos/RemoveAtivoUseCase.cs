using Domain.Interfaces;

namespace Application.UseCases.Ativos;

public class RemoveAtivoUseCase
{
    private readonly IAtivoRepository _ativoRepository;

    public RemoveAtivoUseCase(IAtivoRepository ativoRepository)
    {
        _ativoRepository = ativoRepository;
    }

    public async Task ExecuteAsync(Guid id)
    {
        await _ativoRepository.DeleteAsync(id);
    }
}