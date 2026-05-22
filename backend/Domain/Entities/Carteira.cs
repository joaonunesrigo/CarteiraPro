using Domain.Enums;

namespace Domain.Entities;

public class Carteira
{
    private Carteira()
    {
    }

    public Carteira(Guid usuarioId, string nome, Moeda moeda, string? descricao = null, bool padrao = false)
    {
        UsuarioId = usuarioId;
        Nome = nome.Trim();
        Moeda = moeda;
        Descricao = string.IsNullOrWhiteSpace(descricao) ? null : descricao.Trim();
        Padrao = padrao;
    }

    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid UsuarioId { get; private set; }
    public string Nome { get; private set; } = string.Empty;
    public string? Descricao { get; private set; }
    public Moeda Moeda { get; private set; }
    public bool Padrao { get; private set; }
    public DateTime DataCadastro { get; private set; } = DateTime.UtcNow;

    public void Atualizar(string nome, Moeda moeda, string? descricao)
    {
        Nome = nome.Trim();
        Moeda = moeda;
        Descricao = string.IsNullOrWhiteSpace(descricao) ? null : descricao.Trim();
    }

    public void MarcarComoPadrao()
    {
        Padrao = true;
    }

    public void RemoverPadrao()
    {
        Padrao = false;
    }
}
