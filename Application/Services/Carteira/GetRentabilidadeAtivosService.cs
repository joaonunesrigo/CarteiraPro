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
        var ativos = await _ativoRepository.GetAllAsync();
        var resultado = new List<RentabilidadeDto>();

        foreach (var ativo in ativos)
        {
            var cotacao = await _brapiService.GetCotacaoAsync(ativo.Ticker);

            var valorInvestido = ativo.PrecoMedio * ativo.Quantidade;
            var valorAtual = cotacao * ativo.Quantidade;
            var rentabilidadeReais = valorAtual - valorInvestido;
            var rentabilidadePercent = (cotacao - ativo.PrecoMedio) / ativo.PrecoMedio * 100;

            resultado.Add(new RentabilidadeDto
            {
                Ticker = ativo.Ticker,
                PrecoMedio = ativo.PrecoMedio,
                CotacaoAtual = cotacao,
                Quantidade = ativo.Quantidade,
                ValorInvestido = valorInvestido,
                ValorAtual = valorAtual,
                RentabilidadeReais = rentabilidadeReais,
                RentabilidadePercent = Math.Round(rentabilidadePercent, 2)
            });
        }

        return resultado;
    }
}