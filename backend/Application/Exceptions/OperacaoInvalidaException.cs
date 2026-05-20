namespace Application.Exceptions;

public class OperacaoInvalidaException(string mensagem) : Exception(mensagem)
{
}
