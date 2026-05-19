using Application.Services.Ativos.DTOs;
using Domain.Interfaces;

namespace Application.Services.Carteira;

public class GetRentabilidadeAtivosService
{
    private readonly IAtivoRepository _ativoRepository;
    private readonly IBrapiService _brapiService;

    public GetRentabilidadeAtivosService(IAtivoRepository ativoRepository, IBrapiService brapiService)
    {
        _ativoRepository = ativoRepository;
        _brapiService = brapiService;
    }

    public async Task<IEnumerable<RentabilidadeDto>> ExecuteAsync()
    {
        var ativos = (await _ativoRepository.GetAllAsync()).ToList();
        if (ativos.Count == 0)
            return [];

        var cotacoes = await _brapiService.ObterQuotesAsync(ativos.Select(a => a.Ticker));
        var resultado = new List<RentabilidadeDto>(ativos.Count);

        foreach (var ativo in ativos)
        {
            var cotacao = cotacoes.TryGetValue(ativo.Ticker, out var quote)
                ? quote.Cotacao
                : ativo.PrecoMedio;

            var valorInvestido = ativo.PrecoMedio * ativo.Quantidade;
            var valorAtual = cotacao * ativo.Quantidade;
            var rentabilidadeReais = valorAtual - valorInvestido;
            var rentabilidadePercent = ativo.PrecoMedio > 0
                ? (cotacao - ativo.PrecoMedio) / ativo.PrecoMedio * 100
                : 0;

            resultado.Add(new RentabilidadeDto
            {
                Id = ativo.Id,
                Ticker = ativo.Ticker,
                PrecoMedio = ativo.PrecoMedio,
                CotacaoAtual = cotacao,
                Quantidade = ativo.Quantidade,
                ValorInvestido = valorInvestido,
                ValorAtual = valorAtual,
                RentabilidadeReais = rentabilidadeReais,
                RentabilidadePercent = Math.Round(rentabilidadePercent, 2),
            });
        }

        return resultado;
    }
}
