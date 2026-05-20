namespace Application.Services.Operacoes.DTOs;

public record PosicaoAtivoDto(
    decimal Quantidade,
    decimal PrecoMedio,
    decimal ValorInvestido,
    decimal GanhoRealizado);
