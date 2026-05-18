using Application.Services.Ativos;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Ativo;

[ApiController]
[Route("api/[controller]")]
public class AtivosController : ControllerBase
{
    private readonly AddAtivoService _adicionarAtivo;
    private readonly GetAtivosService _obterAtivos;
    private readonly RemoveAtivoService _removerAtivo;

    public AtivosController(AddAtivoService adicionarAtivo, GetAtivosService obterAtivos, RemoveAtivoService removerAtivo)
    {
        _adicionarAtivo = adicionarAtivo;
        _obterAtivos = obterAtivos;
        _removerAtivo = removerAtivo;
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
        await _adicionarAtivo.ExecuteAsync(request.Ticker, request.Nome, request.PrecoMedio, request.Quantidade, request.Tipo);

        return Created();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _removerAtivo.ExecuteAsync(id);
        return NoContent();
    }
}

