using Domain.Enums;

namespace Application.Dtos;

public record ProventoDto(
    Guid Id,
    Guid? AtivoId,
    string Ticker,
    decimal ValorPorCota,
    decimal Quantidade,
    decimal ValorTotal,
    DateTime DataPagamento,
    TipoProvento Tipo);

public record LinhaImportacaoProventoB3Dto(
    string Ticker,
    string? Produto,
    string Movimentacao,
    decimal Quantidade,
    decimal ValorPorCota,
    decimal ValorTotal,
    DateTime DataPagamento,
    TipoProvento Tipo,
    bool AtivoCadastrado,
    bool JaImportado,
    string OrigemAba);

public record ImportarProventoItemDto(
    string Ticker,
    decimal ValorPorCota,
    decimal Quantidade,
    DateTime DataPagamento,
    TipoProvento Tipo);

public record ImportarProventosResultadoDto(
    int Importados,
    int Ignorados,
    IReadOnlyList<ErroImportacaoDto> Erros);

public record ResumoProventosDto(
    decimal TotalRecebido,
    IReadOnlyList<ResumoProventosMesDto> PorMes,
    IReadOnlyList<ResumoProventosAtivoDto> PorAtivo);

public record ResumoProventosMesDto(string Mes, decimal Total);

public record ResumoProventosAtivoDto(string Ticker, decimal Total);
