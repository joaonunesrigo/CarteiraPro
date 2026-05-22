using Application.Services.Ativos.DTOs;
using Application.Services.Carteiras;
using Application.Services.Operacoes;
using Domain.Interfaces;

namespace Application.Services.Ativos;

public class GetAtivosService
{
    private readonly IAtivoRepository _ativoRepository;
    private readonly CalcularPosicaoAtivoService _calcularPosicao;
    private readonly GetCarteiraAtualService _getCarteiraAtual;

    public GetAtivosService(
        IAtivoRepository ativoRepository,
        CalcularPosicaoAtivoService calcularPosicao,
        GetCarteiraAtualService getCarteiraAtual)
    {
        _ativoRepository = ativoRepository;
        _calcularPosicao = calcularPosicao;
        _getCarteiraAtual = getCarteiraAtual;
    }

    public async Task<IEnumerable<AtivoDto>> ExecuteAsync(Guid? carteiraId = null)
    {
        var carteira = await _getCarteiraAtual.ExecuteAsync(carteiraId);
        var ativos = (await _ativoRepository.GetAllAsync(carteira.Id)).ToList();
        var posicoes = await _calcularPosicao.ExecuteAsync(ativos);

        return ativos.Select(ativo =>
        {
            var posicao = posicoes[ativo.Id];

            return new AtivoDto(
                ativo.Id,
                ativo.Ticker,
                ativo.Nome,
                posicao.PrecoMedio,
                posicao.Quantidade,
                ativo.Tipo,
                ativo.DataCadastro,
                posicao.ValorInvestido);
        });
    }
}