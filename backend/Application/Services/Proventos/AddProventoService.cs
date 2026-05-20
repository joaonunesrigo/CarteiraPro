using Application.Exceptions;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;

namespace Application.Services.Proventos;

public class AddProventoService
{
    private readonly IAtivoRepository _ativoRepository;
    private readonly IProventoRepository _proventoRepository;

    public AddProventoService(IAtivoRepository ativoRepository, IProventoRepository proventoRepository)
    {
        _ativoRepository = ativoRepository;
        _proventoRepository = proventoRepository;
    }

    public async Task ExecuteAsync(Guid ativoId, decimal valorPorCota, decimal quantidade, DateTime dataPagamento, TipoProvento tipo)
    {
        var ativo = await _ativoRepository.GetByIdAsync(ativoId);
        if (ativo is null)
            throw new AtivoNaoEncontradoException(ativoId);

        if (valorPorCota <= 0 || quantidade <= 0)
            throw new ArgumentException("Valor por cota e quantidade devem ser maiores que zero.");

        var provento = new Provento(
            ativoId,
            ativo.Ticker,
            valorPorCota,
            quantidade,
            DateTime.SpecifyKind(dataPagamento.Date, DateTimeKind.Utc),
            tipo);

        await _proventoRepository.AddAsync(provento);
    }
}
