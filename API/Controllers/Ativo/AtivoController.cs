using Application.UseCases.Ativos;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AtivosController : ControllerBase
{
    private readonly AddAtivoUseCase _adicionarAtivo;
    private readonly GetAtivosUseCase _obterAtivos;
    private readonly RemoveAtivoUseCase _removerAtivo;

    public AtivosController(AddAtivoUseCase adicionarAtivo, GetAtivosUseCase obterAtivos, RemoveAtivoUseCase removerAtivo)
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
        await _adicionarAtivo.ExecuteAsync(
            request.Ticker,
            request.Nome,
            request.PrecoMedio,
            request.Quantidade,
            request.Tipo);

        return Created();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _removerAtivo.ExecuteAsync(id);
        return NoContent();
    }
}

