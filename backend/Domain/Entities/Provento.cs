using Domain.Enums;

namespace Domain.Entities;

public class Provento(Guid ativoId, decimal valorPorCota, decimal quantidade, DateTime dataPagamento, TipoProvento tipo)
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid AtivoId { get; private set; } = ativoId;
    public decimal ValorPorCota { get; private set; } = valorPorCota;
    public decimal Quantidade { get; private set; } = quantidade;
    public DateTime DataPagamento { get; private set; } = dataPagamento;
    public TipoProvento Tipo { get; private set; } = tipo;
    public decimal ValorTotal => ValorPorCota * Quantidade;
}