using Domain.Enums;

namespace Domain.Entities;

public class Operacao
{
    private Operacao()
    {
    }

    public Operacao(
        Guid usuarioId,
        Guid ativoId,
        TipoOperacao tipo,
        DateTime data,
        decimal quantidade,
        decimal precoUnitario,
        decimal taxas = 0,
        string? observacao = null)
    {
        UsuarioId = usuarioId;
        AtivoId = ativoId;
        Tipo = tipo;
        Data = DateTime.SpecifyKind(data.Date, DateTimeKind.Utc);
        Quantidade = quantidade;
        PrecoUnitario = precoUnitario;
        Taxas = taxas;
        Observacao = observacao;
    }

    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid UsuarioId { get; private set; }
    public Guid AtivoId { get; private set; }
    public TipoOperacao Tipo { get; private set; }
    public DateTime Data { get; private set; }
    public decimal Quantidade { get; private set; }
    public decimal PrecoUnitario { get; private set; }
    public decimal Taxas { get; private set; }
    public string? Observacao { get; private set; }
    public DateTime DataCadastro { get; private set; } = DateTime.UtcNow;

    public decimal ValorBruto => Quantidade * PrecoUnitario;

    public decimal ValorLiquido => Tipo == TipoOperacao.Compra
        ? ValorBruto + Taxas
        : ValorBruto - Taxas;
}
