using API.Controllers.Ativo.Records;
using Application.Dtos;
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
    private readonly RemoveAllAtivosService _removerTodosAtivos;
    private readonly GetCotacaoAtivoService _getCotacao;
    private readonly PreviewImportacaoB3Service _previewImportacaoB3;
    private readonly ImportarAtivosService _importarAtivos;

    public AtivosController(
        AddAtivoService adicionarAtivo,
        GetAtivosService obterAtivos,
        RemoveAtivoService removerAtivo,
        RemoveAllAtivosService removerTodosAtivos,
        GetCotacaoAtivoService getCotacao,
        PreviewImportacaoB3Service previewImportacaoB3,
        ImportarAtivosService importarAtivos)
    {
        _adicionarAtivo = adicionarAtivo;
        _obterAtivos = obterAtivos;
        _removerAtivo = removerAtivo;
        _removerTodosAtivos = removerTodosAtivos;
        _getCotacao = getCotacao;
        _previewImportacaoB3 = previewImportacaoB3;
        _importarAtivos = importarAtivos;
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

    [HttpDelete("todos")]
    public async Task<IActionResult> DeleteAll()
    {
        var removidos = await _removerTodosAtivos.ExecuteAsync();
        return Ok(new { removidos });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _removerAtivo.ExecuteAsync(id);
        return NoContent();
    }

    [HttpPost("importar/preview")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> PreviewImportacao(IFormFile arquivo)
    {
        if (arquivo is null || arquivo.Length == 0)
            return BadRequest(new { mensagem = "Envie um arquivo .xlsx de posição da B3." });

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
    public async Task<IActionResult> Importar([FromBody] ImportarAtivosRequestRecord request)
    {
        if (request.Ativos is null || request.Ativos.Count == 0)
            return BadRequest(new { mensagem = "Nenhum ativo para importar." });

        var resultado = await _importarAtivos.ExecuteAsync(request.Ativos, request.IgnorarDuplicados);
        return Ok(resultado);
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