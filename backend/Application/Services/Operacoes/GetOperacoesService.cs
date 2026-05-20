using Application.Services.Operacoes.DTOs;
using Domain.Interfaces;

namespace Application.Services.Operacoes;

public class GetOperacoesService
{
    private readonly IOperacaoRepository _operacaoRepository;

    public GetOperacoesService(IOperacaoRepository operacaoRepository)
    {
        _operacaoRepository = operacaoRepository;
    }

    public async Task<IEnumerable<OperacaoDto>> ExecuteAsync(Guid ativoId)
    {
        var operacoes = await _operacaoRepository.GetByAtivoIdAsync(ativoId);

        return operacoes.Select(o => new OperacaoDto(
            o.Id,
            o.AtivoId,
            o.Tipo,
            o.Data,
            o.Quantidade,
            o.PrecoUnitario,
            o.Taxas,
            o.ValorBruto,
            o.ValorLiquido,
            o.Observacao));
    }
}
