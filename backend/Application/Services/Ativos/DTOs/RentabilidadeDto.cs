using Domain.Enums;

namespace Application.Services.Ativos.DTOs;

public class RentabilidadeDto
{
    public Guid Id { get; set; }
    public string Ticker { get; set; } = string.Empty;
    public TipoAtivo Tipo { get; set; }
    public string? Setor { get; set; }
    public decimal PrecoMedio { get; set; }
    public decimal CotacaoAtual { get; set; }
    public decimal Quantidade { get; set; }
    public decimal ValorInvestido { get; set; }
    public decimal ValorAtual { get; set; }
    public decimal RentabilidadePercent { get; set; }
    public decimal RentabilidadeReais { get; set; }
    public DateTime? CotacaoAtualizadaEm { get; set; }
}