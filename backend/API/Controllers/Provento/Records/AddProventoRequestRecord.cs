using Domain.Enums;

namespace API.Controllers.Provento.Records;

public record AddProventoRequestRecord(
    Guid AtivoId,
    decimal ValorPorCota,
    decimal Quantidade,
    DateTime DataPagamento,
    TipoProvento Tipo);
