using Domain.Models;

namespace Domain.Interfaces;

public interface IB3MovimentacaoParser
{
    IReadOnlyList<LinhaMovimentacaoB3> Parse(Stream arquivo);
}
