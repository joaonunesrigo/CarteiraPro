using Domain.Enums;

namespace Domain.Models;

public class LinhaMovimentacaoB3
{
    public string Ticker { get; set; } = string.Empty;
    public string? Produto { get; set; }
    public string Movimentacao { get; set; } = string.Empty;
    public decimal Quantidade { get; set; }
    public decimal ValorPorCota { get; set; }
    public decimal ValorTotal { get; set; }
    public DateTime DataPagamento { get; set; }
    public TipoProvento Tipo { get; set; }
    public string OrigemAba { get; set; } = string.Empty;
}
