using Domain.Interfaces;

namespace Application.Services.Proventos;

public class RemoveProventoService
{
    private readonly IProventoRepository _proventoRepository;

    public RemoveProventoService(IProventoRepository proventoRepository)
    {
        _proventoRepository = proventoRepository;
    }

    public async Task ExecuteAsync(Guid id)
    {
        await _proventoRepository.DeleteAsync(id);
    }
}
