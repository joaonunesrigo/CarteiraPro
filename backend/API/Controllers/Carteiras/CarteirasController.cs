using API.Controllers.Carteiras.Records;
using Application.Services.Carteiras;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Carteiras;

[ApiController]
[Route("api/[controller]")]
public class CarteirasController : ControllerBase
{
    private readonly GetCarteirasService _getCarteiras;
    private readonly AddCarteiraService _addCarteira;
    private readonly UpdateCarteiraService _updateCarteira;
    private readonly RemoveCarteiraService _removeCarteira;

    public CarteirasController(
        GetCarteirasService getCarteiras,
        AddCarteiraService addCarteira,
        UpdateCarteiraService updateCarteira,
        RemoveCarteiraService removeCarteira)
    {
        _getCarteiras = getCarteiras;
        _addCarteira = addCarteira;
        _updateCarteira = updateCarteira;
        _removeCarteira = removeCarteira;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var carteiras = await _getCarteiras.ExecuteAsync();
        return Ok(carteiras);
    }

    [HttpPost]
    public async Task<IActionResult> Add([FromBody] CarteiraRequestRecord request)
    {
        try
        {
            var id = await _addCarteira.ExecuteAsync(request.Nome, request.Moeda, request.Descricao, request.Padrao);
            return Created($"/api/carteiras/{id}", new { id });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] CarteiraRequestRecord request)
    {
        try
        {
            await _updateCarteira.ExecuteAsync(id, request.Nome, request.Moeda, request.Descricao, request.Padrao);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { mensagem = ex.Message });
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await _removeCarteira.ExecuteAsync(id);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { mensagem = ex.Message });
        }
    }
}
