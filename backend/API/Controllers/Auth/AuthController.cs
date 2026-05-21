using API.Auth;
using API.Controllers.Auth.Records;
using Application.Dtos;
using Application.Exceptions;
using Application.Services.Auth;
using Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Auth;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly RegistrarUsuarioService _registrarUsuario;
    private readonly LoginUsuarioService _loginUsuario;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ICurrentUser _currentUser;
    private readonly JwtTokenService _jwtTokenService;

    public AuthController(
        RegistrarUsuarioService registrarUsuario,
        LoginUsuarioService loginUsuario,
        IUsuarioRepository usuarioRepository,
        ICurrentUser currentUser,
        JwtTokenService jwtTokenService)
    {
        _registrarUsuario = registrarUsuario;
        _loginUsuario = loginUsuario;
        _usuarioRepository = usuarioRepository;
        _currentUser = currentUser;
        _jwtTokenService = jwtTokenService;
    }

    [AllowAnonymous]
    [HttpPost("registrar")]
    public async Task<IActionResult> Registrar([FromBody] RegistrarRequestRecord request)
    {
        try
        {
            var usuario = await _registrarUsuario.ExecuteAsync(request.Nome, request.Email, request.Senha);
            return Ok(CriarResposta(usuario));
        }
        catch (EmailJaCadastradoException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestRecord request)
    {
        try
        {
            var usuario = await _loginUsuario.ExecuteAsync(request.Email, request.Senha);
            return Ok(CriarResposta(usuario));
        }
        catch (CredenciaisInvalidasException ex)
        {
            return Unauthorized(new { mensagem = ex.Message });
        }
    }

    [HttpGet("me")]
    public async Task<IActionResult> Me()
    {
        if (_currentUser.UsuarioId is not Guid usuarioId)
            return Unauthorized();

        var usuario = await _usuarioRepository.GetByIdAsync(usuarioId);
        if (usuario is null)
            return Unauthorized();

        return Ok(new UsuarioAutenticadoDto(usuario.Id, usuario.Nome, usuario.Email));
    }

    private object CriarResposta(UsuarioAutenticadoDto usuario)
    {
        return new
        {
            token = _jwtTokenService.GerarToken(usuario),
            usuario,
        };
    }
}
