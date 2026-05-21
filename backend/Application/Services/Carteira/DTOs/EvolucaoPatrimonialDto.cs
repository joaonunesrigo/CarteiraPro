namespace Application.Services.Carteira.DTOs;

public class EvolucaoPatrimonialDto
{
    public IReadOnlyList<EvolucaoPatrimonialPontoDto> Pontos { get; set; } = [];
    public IReadOnlyList<string> TickersSemHistorico { get; set; } = [];
}

public class EvolucaoPatrimonialPontoDto
{
    public DateTime Data { get; set; }
    public decimal Patrimonio { get; set; }
    public decimal ValorInvestido { get; set; }
}
