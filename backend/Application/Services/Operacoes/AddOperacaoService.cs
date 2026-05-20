using Application.Exceptions;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;

namespace Application.Services.Operacoes;

public class AddOperacaoService
{
    private readonly IAtivoRepository _ativoRepository;
    private readonly IOperacaoRepository _operacaoRepository;

    public AddOperacaoService(IAtivoRepository ativoRepository, IOperacaoRepository operacaoRepository)
    {
        _ativoRepository = ativoRepository;
        _operacaoRepository = operacaoRepository;
    }

    public async Task ExecuteAsync(
        Guid ativoId,
        TipoOperacao tipo,
        DateTime data,
        decimal quantidade,
        decimal precoUnitario,
        decimal taxas = 0,
        string? observacao = null)
    {
        var ativo = await _ativoRepository.GetByIdAsync(ativoId);
        if (ativo is null)
            throw new AtivoNaoEncontradoException(ativoId);

        var operacao = new Operacao(ativoId, tipo, data, quantidade, precoUnitario, taxas, observacao);
        var operacoes = (await _operacaoRepository.GetByAtivoIdAsync(ativoId)).Append(operacao);

        CalcularPosicaoAtivoService.Calcular(operacoes);

        await _operacaoRepository.AddAsync(operacao);
    }
}
