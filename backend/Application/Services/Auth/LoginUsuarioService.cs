using Application.Dtos;
using Application.Exceptions;
using Domain.Interfaces;

namespace Application.Services.Auth;

public class LoginUsuarioService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly PasswordHasher _passwordHasher;

    public LoginUsuarioService(IUsuarioRepository usuarioRepository, PasswordHasher passwordHasher)
    {
        _usuarioRepository = usuarioRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<UsuarioAutenticadoDto> ExecuteAsync(string email, string senha)
    {
        var emailNormalizado = email.Trim().ToLowerInvariant();
        var usuario = await _usuarioRepository.GetByEmailAsync(emailNormalizado);

        if (usuario is null || !_passwordHasher.Verify(senha, usuario.SenhaHash))
            throw new CredenciaisInvalidasException();

        return new UsuarioAutenticadoDto(usuario.Id, usuario.Nome, usuario.Email);
    }
}
