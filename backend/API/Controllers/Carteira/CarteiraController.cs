using Application.Services.Ativos;
using Application.Services.Carteira;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CarteiraController : ControllerBase
{
    private readonly GetResumoCarteiraService _getResumoCarteira;
    private readonly GetRentabilidadeAtivosService _getRentabilidade;

    public CarteiraController(
        GetResumoCarteiraService getResumoCarteira,
        GetRentabilidadeAtivosService getRentabilidade)
    {
        _getResumoCarteira = getResumoCarteira;
        _getRentabilidade = getRentabilidade;
    }

    [HttpGet("resumo")]
    public async Task<IActionResult> GetResumo()
    {
        var resumo = await _getResumoCarteira.ExecuteAsync();
        return Ok(resumo);
    }

    [HttpGet("rentabilidade")]
    public async Task<IActionResult> GetRentabilidade()
    {
        var rentabilidade = await _getRentabilidade.ExecuteAsync();
        return Ok(rentabilidade);
    }
}