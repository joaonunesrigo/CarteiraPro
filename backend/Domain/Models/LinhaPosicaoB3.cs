using Domain.Enums;

namespace Domain.Models;

public class LinhaPosicaoB3
{
    public required string Ticker { get; init; }
    public string? Produto { get; init; }
    public decimal Quantidade { get; init; }
    public decimal? PrecoFechamento { get; init; }
    public TipoAtivo Tipo { get; init; }
    public required string OrigemAba { get; init; }
}
