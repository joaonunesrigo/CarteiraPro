using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.Dtos;
using Microsoft.IdentityModel.Tokens;

namespace API.Auth;

public class JwtTokenService
{
    private readonly IConfiguration _configuration;

    public JwtTokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GerarToken(UsuarioAutenticadoDto usuario)
    {
        var jwt = _configuration.GetSection("Jwt");
        var chave = jwt.GetValue<string>("Key")
            ?? throw new InvalidOperationException("Configure Jwt:Key.");
        var issuer = jwt.GetValue<string>("Issuer") ?? "CarteiraPro";
        var audience = jwt.GetValue<string>("Audience") ?? "CarteiraPro";
        var expiraEmHoras = jwt.GetValue<int?>("ExpiresHours") ?? 12;

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new Claim(ClaimTypes.Name, usuario.Nome),
            new Claim(ClaimTypes.Email, usuario.Email),
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(chave));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer,
            audience,
            claims,
            expires: DateTime.UtcNow.AddHours(expiraEmHoras),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
