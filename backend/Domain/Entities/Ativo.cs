using Domain.Enums;

namespace Domain.Entities;

public class Ativo
{
    private Ativo()
    {
    }

    public Ativo(Guid usuarioId, string ticker, string nome, TipoAtivo tipo)
    {
        UsuarioId = usuarioId;
        Ticker = ticker.ToUpper();
        Nome = nome;
        Tipo = tipo;
    }

    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid UsuarioId { get; private set; }
    public string Ticker { get; private set; } = string.Empty;
    public string Nome { get; private set; } = string.Empty;
    public TipoAtivo Tipo { get; private set; }
    public DateTime DataCadastro { get; private set; } = DateTime.UtcNow;
}