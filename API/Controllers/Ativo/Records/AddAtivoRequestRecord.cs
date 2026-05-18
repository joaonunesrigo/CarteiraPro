using Domain.Enums;

public record AddAtivoRequestRecord(
    string Ticker,
    decimal PrecoMedio,
    decimal Quantidade,
    TipoAtivo Tipo);