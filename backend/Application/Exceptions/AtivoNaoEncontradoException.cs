namespace Application.Exceptions;

public class AtivoNaoEncontradoException : Exception
{
    public AtivoNaoEncontradoException(string ticker)
        : base($"Ativo {ticker} não encontrado na carteira.")
    {
    }

    public AtivoNaoEncontradoException(Guid id)
        : base($"Ativo {id} não encontrado.")
    {
    }
}
