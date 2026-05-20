using Application.Services.Ativos.DTOs;
using Application.Services.Operacoes;
using Domain.Interfaces;

namespace Application.Services.Ativos;

public class GetAtivosService
{
    private readonly IAtivoRepository _ativoRepository;
    private readonly CalcularPosicaoAtivoService _calcularPosicao;

    public GetAtivosService(IAtivoRepository ativoRepository, CalcularPosicaoAtivoService calcularPosicao)
    {
        _ativoRepository = ativoRepository;
        _calcularPosicao = calcularPosicao;
    }

    public async Task<IEnumerable<AtivoDto>> ExecuteAsync()
    {
        var ativos = (await _ativoRepository.GetAllAsync()).ToList();
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