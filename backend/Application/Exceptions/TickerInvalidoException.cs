namespace Application.Exceptions;

public class TickerInvalidoException(string ticker) : Exception($"O ticker '{ticker.ToUpper()}' não foi encontrado. Verifique e tente novamente.")
{
    public string Ticker { get; } = ticker.ToUpper();
}
