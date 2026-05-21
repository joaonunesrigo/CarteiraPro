using Application.Dtos;
using Application.Exceptions;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services.Auth;

public class RegistrarUsuarioService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly PasswordHasher _passwordHasher;

    public RegistrarUsuarioService(IUsuarioRepository usuarioRepository, PasswordHasher passwordHasher)
    {
        _usuarioRepository = usuarioRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<UsuarioAutenticadoDto> ExecuteAsync(string nome, string email, string senha)
    {
        var emailNormalizado = email.Trim().ToLowerInvariant();

        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("Informe seu nome.");

        if (string.IsNullOrWhiteSpace(emailNormalizado) || !emailNormalizado.Contains('@'))
            throw new ArgumentException("Informe um e-mail válido.");

        if (string.IsNullOrWhiteSpace(senha) || senha.Length < 8)
            throw new ArgumentException("A senha deve ter pelo menos 8 caracteres.");

        if (await _usuarioRepository.ExistsByEmailAsync(emailNormalizado))
            throw new EmailJaCadastradoException(emailNormalizado);

        var usuario = new Usuario(nome, emailNormalizado, _passwordHasher.Hash(senha));
        await _usuarioRepository.AddAsync(usuario);

        return new UsuarioAutenticadoDto(usuario.Id, usuario.Nome, usuario.Email);
    }
}
