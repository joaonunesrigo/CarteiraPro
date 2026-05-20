using Domain.Enums;

namespace Application.Services.Operacoes.DTOs;

public record OperacaoDto(
    Guid Id,
    Guid AtivoId,
    TipoOperacao Tipo,
    DateTime Data,
    decimal Quantidade,
    decimal PrecoUnitario,
    decimal Taxas,
    decimal ValorBruto,
    decimal ValorLiquido,
    string? Observacao);
