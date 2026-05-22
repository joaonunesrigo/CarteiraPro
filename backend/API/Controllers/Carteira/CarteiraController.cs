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
    private readonly GetEvolucaoPatrimonialService _getEvolucao;

    public CarteiraController(
        GetResumoCarteiraService getResumoCarteira,
        GetRentabilidadeAtivosService getRentabilidade,
        GetEvolucaoPatrimonialService getEvolucao)
    {
        _getResumoCarteira = getResumoCarteira;
        _getRentabilidade = getRentabilidade;
        _getEvolucao = getEvolucao;
    }

    [HttpGet("resumo")]
    public async Task<IActionResult> GetResumo([FromQuery] Guid? carteiraId)
    {
        var resumo = await _getResumoCarteira.ExecuteAsync(carteiraId);
        return Ok(resumo);
    }

    [HttpGet("rentabilidade")]
    public async Task<IActionResult> GetRentabilidade([FromQuery] Guid? carteiraId)
    {
        var rentabilidade = await _getRentabilidade.ExecuteAsync(carteiraId);
        return Ok(rentabilidade);
    }

    [HttpGet("evolucao")]
    public async Task<IActionResult> GetEvolucao([FromQuery] int meses = 12, [FromQuery] Guid? carteiraId = null)
    {
        var evolucao = await _getEvolucao.ExecuteAsync(meses, carteiraId);
        return Ok(evolucao);
    }
}