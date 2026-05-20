using Domain.Models;

namespace Domain.Interfaces;

public interface IB3PosicaoParser
{
    IReadOnlyList<LinhaPosicaoB3> Parse(Stream arquivo);
}
