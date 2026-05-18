using Domain.Enums;

public record AddAtivoRequestRecord(
    string Ticker,
    string Nome,
    decimal PrecoMedio,
    decimal Quantidade,
    TipoAtivo Tipo);