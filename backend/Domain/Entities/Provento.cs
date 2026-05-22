using Domain.Enums;

namespace Domain.Entities;

public class Provento(
    Guid usuarioId,
    Guid carteiraId,
    Guid? ativoId,
    string ticker,
    decimal valorPorCota,
    decimal quantidade,
    DateTime dataPagamento,
    TipoProvento tipo)
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid UsuarioId { get; private set; } = usuarioId;
    public Guid CarteiraId { get; private set; } = carteiraId;
    public Guid? AtivoId { get; private set; } = ativoId;
    public string Ticker { get; private set; } = ticker.Trim().ToUpperInvariant();
    public decimal ValorPorCota { get; private set; } = valorPorCota;
    public decimal Quantidade { get; private set; } = quantidade;
    public DateTime DataPagamento { get; private set; } = dataPagamento;
    public TipoProvento Tipo { get; private set; } = tipo;
    public decimal ValorTotal => ValorPorCota * Quantidade;
}