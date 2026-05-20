using Domain.Enums;

namespace Application.Services.Ativos.DTOs;

public record AtivoDto(
    Guid Id,
    string Ticker,
    string Nome,
    decimal PrecoMedio,
    decimal Quantidade,
    TipoAtivo Tipo,
    DateTime DataCadastro,
    decimal ValorInvestido);
