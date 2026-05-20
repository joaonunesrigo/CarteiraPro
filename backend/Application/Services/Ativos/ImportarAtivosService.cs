using Application.Dtos;
using Application.Exceptions;
using Domain.Interfaces;

namespace Application.Services.Ativos;

public class ImportarAtivosService
{
    private readonly AddAtivoService _adicionarAtivo;
    private readonly IAtivoRepository _ativoRepository;

    public ImportarAtivosService(AddAtivoService adicionarAtivo, IAtivoRepository ativoRepository)
    {
        _adicionarAtivo = adicionarAtivo;
        _ativoRepository = ativoRepository;
    }

    public async Task<ImportarAtivosResultadoDto> ExecuteAsync(
        IReadOnlyList<ImportarAtivoItemDto> ativos,
        bool ignorarDuplicados = true)
    {
        var erros = new List<ErroImportacaoDto>();
        var importados = 0;
        var ignorados = 0;

        foreach (var item in ativos)
        {
            var ticker = item.Ticker.Trim().ToUpperInvariant();

            if (string.IsNullOrWhiteSpace(ticker))
                continue;

            if (item.Quantidade <= 0)
            {
                erros.Add(new ErroImportacaoDto(ticker, "Quantidade deve ser maior que zero."));
                continue;
            }

            if (item.PrecoMedio < 0)
            {
                erros.Add(new ErroImportacaoDto(ticker, "Preço médio inválido."));
                continue;
            }

            var existente = await _ativoRepository.GetByTickerAsync(ticker);
            if (existente is not null)
            {
                if (ignorarDuplicados)
                {
                    ignorados++;
                    continue;
                }
            }

            try
            {
                await _adicionarAtivo.ExecuteAsync(
                    ticker,
                    item.PrecoMedio,
                    item.Quantidade,
                    item.Tipo);
                importados++;
            }
            catch (TickerJaCadastradoException)
            {
                ignorados++;
            }
            catch (TickerInvalidoException ex)
            {
                erros.Add(new ErroImportacaoDto(ticker, ex.Message));
            }
            catch (Exception ex)
            {
                erros.Add(new ErroImportacaoDto(ticker, ex.Message));
            }
        }

        return new ImportarAtivosResultadoDto(importados, ignorados, erros);
    }
}
