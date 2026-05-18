using Domain.Enums;

namespace Domain.Entities;

public class Ativo(string ticker, string nome, decimal precoMedio, decimal quantidade, TipoAtivo tipo)
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Ticker { get; private set; } = ticker.ToUpper();
    public string Nome { get; private set; } = nome;
    public decimal PrecoMedio { get; private set; } = precoMedio;
    public decimal Quantidade { get; private set; } = quantidade;
    public TipoAtivo Tipo { get; private set; } = tipo;
    public DateTime DataCadastro { get; private set; } = DateTime.UtcNow;
    public decimal ValorInvestido => PrecoMedio * Quantidade;
}