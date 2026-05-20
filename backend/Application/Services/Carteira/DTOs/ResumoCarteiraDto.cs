namespace Application.Services.Carteira.DTOs;

public class ResumoCarteiraDto
{
    public decimal TotalInvestido { get; set; }
    public decimal TotalAtual { get; set; }
    public decimal RentabilidadeReais { get; set; }
    public decimal RentabilidadePercent { get; set; }
}