namespace Application.Exceptions;

public class TickerJaCadastradoException(string ticker)
    : Exception($"O ticker '{ticker.ToUpper()}' já está cadastrado na carteira.")
{
    public string Ticker { get; } = ticker.ToUpper();
}
