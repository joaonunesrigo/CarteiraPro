using Application.Services.Ativos;
using Application.Services.Carteira;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Ativo;

[ApiController]
[Route("api/[controller]")]
public class AtivosController : ControllerBase
{
    private readonly AddAtivoService _adicionarAtivo;
    private readonly GetAtivosService _obterAtivos;
    private readonly RemoveAtivoService _removerAtivo;
    private readonly GetCotacaoAtivoService _getCotacao;
    private readonly GetRentabilidadeAtivosService _getRentabilidade;

    public AtivosController(AddAtivoService adicionarAtivo, GetAtivosService obterAtivos, RemoveAtivoService removerAtivo, GetCotacaoAtivoService getCotacao, GetRentabilidadeAtivosService getRentabilidade)
    {
        _adicionarAtivo = adicionarAtivo;
        _obterAtivos = obterAtivos;
        _removerAtivo = removerAtivo;
        _getCotacao = getCotacao;
        _getRentabilidade = getRentabilidade;

    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var ativos = await _obterAtivos.ExecuteAsync();
        return Ok(ativos);
    }

    [HttpPost]
    public async Task<IActionResult> Add([FromBody] AddAtivoRequestRecord request)
    {
        await _adicionarAtivo.ExecuteAsync(request.Ticker, request.PrecoMedio, request.Quantidade, request.Tipo);

        return Created();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _removerAtivo.ExecuteAsync(id);
        return NoContent();
    }

    [HttpGet("{ticker}/cotacao")]
    public async Task<IActionResult> GetCotacao(string ticker)
    {
        var cotacao = await _getCotacao.ExecuteAsync(ticker);
        return Ok(new { ticker = ticker.ToUpper(), cotacao });
    }
}