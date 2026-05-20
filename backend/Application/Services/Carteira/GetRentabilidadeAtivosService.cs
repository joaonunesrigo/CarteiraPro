using Application.Services.Ativos.DTOs;
using Application.Services.Operacoes;
using Domain.Interfaces;

namespace Application.Services.Carteira;

public class GetRentabilidadeAtivosService
{
    private readonly IAtivoRepository _ativoRepository;
    private readonly IBrapiService _brapiService;
    private readonly CalcularPosicaoAtivoService _calcularPosicao;

    public GetRentabilidadeAtivosService(
        IAtivoRepository ativoRepository,
        IBrapiService brapiService,
        CalcularPosicaoAtivoService calcularPosicao)
    {
        _ativoRepository = ativoRepository;
        _brapiService = brapiService;
        _calcularPosicao = calcularPosicao;
    }

    public async Task<IEnumerable<RentabilidadeDto>> ExecuteAsync()
    {
        var ativos = (await _ativoRepository.GetAllAsync()).ToList();
        if (ativos.Count == 0)
            return [];

        var cotacoes = await _brapiService.ObterQuotesAsync(ativos.Select(a => a.Ticker));
        var posicoes = await _calcularPosicao.ExecuteAsync(ativos);
        var resultado = new List<RentabilidadeDto>(ativos.Count);

        foreach (var ativo in ativos)
        {
            var posicao = posicoes[ativo.Id];
            if (posicao.Quantidade <= 0)
                continue;

            var cotacao = cotacoes.TryGetValue(ativo.Ticker, out var quote)
                ? quote.Cotacao
                : posicao.PrecoMedio;

            var valorInvestido = posicao.ValorInvestido;
            var valorAtual = cotacao * posicao.Quantidade;
            var rentabilidadeReais = valorAtual - valorInvestido;
            var rentabilidadePercent = posicao.PrecoMedio > 0
                ? (cotacao - posicao.PrecoMedio) / posicao.PrecoMedio * 100
                : 0;

            resultado.Add(new RentabilidadeDto
            {
                Id = ativo.Id,
                Ticker = ativo.Ticker,
                Tipo = ativo.Tipo,
                PrecoMedio = posicao.PrecoMedio,
                CotacaoAtual = cotacao,
                Quantidade = posicao.Quantidade,
                ValorInvestido = valorInvestido,
                ValorAtual = valorAtual,
                RentabilidadeReais = rentabilidadeReais,
                RentabilidadePercent = Math.Round(rentabilidadePercent, 2),
            });
        }

        return resultado;
    }
}
