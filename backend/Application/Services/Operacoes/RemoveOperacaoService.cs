using Application.Exceptions;
using Domain.Interfaces;

namespace Application.Services.Operacoes;

public class RemoveOperacaoService
{
    private readonly IOperacaoRepository _operacaoRepository;

    public RemoveOperacaoService(IOperacaoRepository operacaoRepository)
    {
        _operacaoRepository = operacaoRepository;
    }

    public async Task ExecuteAsync(Guid id)
    {
        var operacao = await _operacaoRepository.GetByIdAsync(id);
        if (operacao is null)
            return;

        var operacoesRestantes = (await _operacaoRepository.GetByAtivoIdAsync(operacao.AtivoId))
            .Where(o => o.Id != id);

        CalcularPosicaoAtivoService.Calcular(operacoesRestantes);

        await _operacaoRepository.DeleteAsync(operacao);
    }
}
