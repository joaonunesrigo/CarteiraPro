using API.Controllers.Provento.Records;
using Application.Exceptions;
using Application.Services.Proventos;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Provento;

[ApiController]
[Route("api/[controller]")]
public class ProventosController : ControllerBase
{
    private readonly AddProventoService _adicionarProvento;
    private readonly GetProventosService _obterProventos;
    private readonly GetResumoProventosService _obterResumo;
    private readonly RemoveProventoService _removerProvento;
    private readonly PreviewImportacaoProventosB3Service _previewImportacaoB3;
    private readonly ImportarProventosB3Service _importarProventosB3;

    public ProventosController(
        AddProventoService adicionarProvento,
        GetProventosService obterProventos,
        GetResumoProventosService obterResumo,
        RemoveProventoService removerProvento,
        PreviewImportacaoProventosB3Service previewImportacaoB3,
        ImportarProventosB3Service importarProventosB3)
    {
        _adicionarProvento = adicionarProvento;
        _obterProventos = obterProventos;
        _obterResumo = obterResumo;
        _removerProvento = removerProvento;
        _previewImportacaoB3 = previewImportacaoB3;
        _importarProventosB3 = importarProventosB3;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] Guid? ativoId,
        [FromQuery] DateTime? dataInicio,
        [FromQuery] DateTime? dataFim)
    {
        var proventos = await _obterProventos.ExecuteAsync(ativoId, dataInicio, dataFim);
        return Ok(proventos);
    }

    [HttpGet("resumo")]
    public async Task<IActionResult> GetResumo(
        [FromQuery] Guid? ativoId,
        [FromQuery] DateTime? dataInicio,
        [FromQuery] DateTime? dataFim)
    {
        var resumo = await _obterResumo.ExecuteAsync(ativoId, dataInicio, dataFim);
        return Ok(resumo);
    }

    [HttpPost]
    public async Task<IActionResult> Add([FromBody] AddProventoRequestRecord request)
    {
        try
        {
            await _adicionarProvento.ExecuteAsync(
                request.AtivoId,
                request.ValorPorCota,
                request.Quantidade,
                request.DataPagamento,
                request.Tipo);

            return Created();
        }
        catch (AtivoNaoEncontradoException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _removerProvento.ExecuteAsync(id);
        return NoContent();
    }

    [HttpPost("importar/preview")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> PreviewImportacao(IFormFile arquivo)
    {
        if (arquivo is null || arquivo.Length == 0)
            return BadRequest(new { mensagem = "Envie um arquivo .xlsx de movimentação da B3." });

        if (!arquivo.FileName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase))
            return BadRequest(new { mensagem = "Apenas arquivos .xlsx são aceitos." });

        try
        {
            await using var stream = arquivo.OpenReadStream();
            var linhas = await _previewImportacaoB3.ExecuteAsync(stream);
            return Ok(new { linhas });
        }
        catch (ArquivoB3InvalidoException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    [HttpPost("importar")]
    public async Task<IActionResult> Importar([FromBody] ImportarProventosRequestRecord request)
    {
        if (request.Proventos is null || request.Proventos.Count == 0)
            return BadRequest(new { mensagem = "Nenhum provento para importar." });

        var resultado = await _importarProventosB3.ExecuteAsync(request.Proventos, request.IgnorarDuplicados);
        return Ok(resultado);
    }
}
