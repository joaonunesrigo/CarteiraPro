namespace Domain.Models;

public class BrapiQuote
{
    public string Nome { get; set; } = string.Empty;
    public decimal Cotacao { get; set; }
    public DateTime? RegularMarketTime { get; set; }
}
