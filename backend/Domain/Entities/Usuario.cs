namespace Domain.Entities;

public class Usuario
{
    private Usuario()
    {
    }

    public Usuario(string nome, string email, string senhaHash)
    {
        Nome = nome.Trim();
        Email = email.Trim().ToLowerInvariant();
        SenhaHash = senhaHash;
    }

    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Nome { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string SenhaHash { get; private set; } = string.Empty;
    public DateTime DataCadastro { get; private set; } = DateTime.UtcNow;
}
