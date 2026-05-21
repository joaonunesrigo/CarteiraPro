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

    [HttpGet("evolucao")]
    public async Task<IActionResult> GetEvolucao([FromQuery] int meses = 12)
    {
        var evolucao = await _getEvolucao.ExecuteAsync(meses);
        return Ok(evolucao);
    }
}