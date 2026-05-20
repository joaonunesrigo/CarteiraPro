using Application.Exceptions;
using Application.Services.Operacoes;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Operacao;

[ApiController]
[Route("api/[controller]")]
public class OperacoesController : ControllerBase
{
    private readonly RemoveOperacaoService _removerOperacao;

    public OperacoesController(RemoveOperacaoService removerOperacao)
    {
        _removerOperacao = removerOperacao;
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await _removerOperacao.ExecuteAsync(id);
            return NoContent();
        }
        catch (OperacaoInvalidaException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }
}
