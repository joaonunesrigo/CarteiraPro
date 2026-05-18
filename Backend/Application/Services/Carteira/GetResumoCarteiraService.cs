using Application.Services.Carteira.DTOs;
using Application.Services.Carteira;

namespace Application.Services.Carteira;

public class GetResumoCarteiraService
{
    private readonly GetRentabilidadeAtivosService _getRentabilidade;

    public GetResumoCarteiraService(GetRentabilidadeAtivosService getRentabilidade)
    {
        _getRentabilidade = getRentabilidade;
    }

    public async Task<ResumoCarteiraDto> ExecuteAsync()
    {
        var ativos = await _getRentabilidade.ExecuteAsync();

        var totalInvestido = ativos.Sum(a => a.ValorInvestido);
        var totalAtual = ativos.Sum(a => a.ValorAtual);

        var rentabilidadeReais = totalAtual - totalInvestido;
        var rentabilidadePercent = totalInvestido > 0 ? Math.Round(rentabilidadeReais / totalInvestido * 100, 2) : 0;

        return new ResumoCarteiraDto
        {
            TotalInvestido = totalInvestido,
            TotalAtual = totalAtual,
            RentabilidadeReais = rentabilidadeReais,
            RentabilidadePercent = rentabilidadePercent
        };
    }
}