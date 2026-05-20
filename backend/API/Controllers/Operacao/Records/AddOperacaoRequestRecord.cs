using Domain.Enums;

namespace API.Controllers.Operacao.Records;

public record AddOperacaoRequestRecord(
    TipoOperacao Tipo,
    DateTime Data,
    decimal Quantidade,
    decimal PrecoUnitario,
    decimal Taxas = 0,
    string? Observacao = null);
