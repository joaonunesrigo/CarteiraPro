using Application.Dtos;
using Application.Services.Carteiras;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services.Proventos;

public class ImportarProventosB3Service
{
    private readonly IAtivoRepository _ativoRepository;
    private readonly IProventoRepository _proventoRepository;
    private readonly ICurrentUser _currentUser;
    private readonly GetCarteiraAtualService _getCarteiraAtual;

    public ImportarProventosB3Service(
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

    public async Task<ImportarProventosResultadoDto> ExecuteAsync(
        Guid? carteiraId,
        IReadOnlyList<ImportarProventoItemDto> proventos,
        bool ignorarDuplicados = true)
    {
        var erros = new List<ErroImportacaoDto>();
        var importados = 0;
        var ignorados = 0;
        var usuarioId = _currentUser.UsuarioId
            ?? throw new InvalidOperationException("Usuário autenticado não encontrado.");
        var carteira = await _getCarteiraAtual.ExecuteAsync(carteiraId);

        foreach (var item in proventos)
        {
            var ticker = item.Ticker.Trim().ToUpperInvariant();

            if (string.IsNullOrWhiteSpace(ticker))
                continue;

            if (item.Quantidade <= 0 || item.ValorPorCota <= 0)
            {
                erros.Add(new ErroImportacaoDto(ticker, "Quantidade e valor por cota devem ser maiores que zero."));
                continue;
            }

            var ativo = await _ativoRepository.GetByTickerAsync(ticker, carteira.Id);
            var dataPagamento = DateTime.SpecifyKind(item.DataPagamento.Date, DateTimeKind.Utc);
            var valorPorCota = Math.Round(item.ValorPorCota, 6, MidpointRounding.AwayFromZero);

            var existe = await _proventoRepository.ExistsSimilarAsync(
                carteira.Id,
                ticker,
                dataPagamento,
                valorPorCota,
                item.Quantidade,
                item.Tipo);

            if (existe && ignorarDuplicados)
            {
                ignorados++;
                continue;
            }

            var provento = new Provento(
                usuarioId,
                carteira.Id,
                ativo?.Id,
                ticker,
                valorPorCota,
                item.Quantidade,
                dataPagamento,
                item.Tipo);

            await _proventoRepository.AddAsync(provento);
            importados++;
        }

        return new ImportarProventosResultadoDto(importados, ignorados, erros);
    }
}
