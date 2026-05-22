using Application.Exceptions;
using Application.Services.Carteiras;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;

namespace Application.Services.Proventos;

public class AddProventoService
{
    private readonly IAtivoRepository _ativoRepository;
    private readonly IProventoRepository _proventoRepository;
    private readonly ICurrentUser _currentUser;
    private readonly GetCarteiraAtualService _getCarteiraAtual;

    public AddProventoService(
        IAtivoRepository ativoRepository,
        IProventoRepository proventoRepository,
        ICurrentUser currentUser,
        GetCarteiraAtualService getCarteiraAtual)
    {
        _ativoRepository = ativoRepository;
        _proventoRepository = proventoRepository;
        _currentUser = currentUser;
        _getCarteiraAtual = getCarteiraAtual;
    }

    public async Task ExecuteAsync(Guid? carteiraId, Guid ativoId, decimal valorPorCota, decimal quantidade, DateTime dataPagamento, TipoProvento tipo)
    {
        var carteira = await _getCarteiraAtual.ExecuteAsync(carteiraId);
        var ativo = await _ativoRepository.GetByIdAsync(ativoId);
        if (ativo is null || ativo.CarteiraId != carteira.Id)
            throw new AtivoNaoEncontradoException(ativoId);

        if (valorPorCota <= 0 || quantidade <= 0)
            throw new ArgumentException("Valor por cota e quantidade devem ser maiores que zero.");

        var usuarioId = _currentUser.UsuarioId
            ?? throw new InvalidOperationException("Usuário autenticado não encontrado.");

        var provento = new Provento(
            usuarioId,
            carteira.Id,
            ativoId,
            ativo.Ticker,
            valorPorCota,
            quantidade,
            DateTime.SpecifyKind(dataPagamento.Date, DateTimeKind.Utc),
            tipo);

        await _proventoRepository.AddAsync(provento);
    }
}
