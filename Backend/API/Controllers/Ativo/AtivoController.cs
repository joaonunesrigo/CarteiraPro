using Application.Exceptions;
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

    public AtivosController(AddAtivoService adicionarAtivo, GetAtivosService obterAtivos, RemoveAtivoService removerAtivo, GetCotacaoAtivoService getCotacao, GetRentabilidadeAtivosService getRentabilidade)
    {
        _adicionarAtivo = adicionarAtivo;
        _obterAtivos = obterAtivos;
        _removerAtivo = removerAtivo;
        _getCotacao = getCotacao;
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
        try
        {
            await _adicionarAtivo.ExecuteAsync(
                request.Ticker,
                request.PrecoMedio,
                request.Quantidade,
                request.Tipo);

            return Created();
        }
        catch (TickerJaCadastradoException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
        catch (TickerInvalidoException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
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
        try
        {
            var cotacao = await _getCotacao.ExecuteAsync(ticker);
            return Ok(new { ticker = ticker.ToUpper(), cotacao });
        }
        catch (TickerInvalidoException ex)
        {
            return NotFound(new { mensagem = ex.Message });
        }
    }
}