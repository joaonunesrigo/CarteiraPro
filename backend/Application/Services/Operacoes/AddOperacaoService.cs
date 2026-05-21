using Application.Exceptions;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;

namespace Application.Services.Operacoes;

public class AddOperacaoService
{
    private readonly IAtivoRepository _ativoRepository;
    private readonly IOperacaoRepository _operacaoRepository;
    private readonly ICurrentUser _currentUser;

    public AddOperacaoService(
        IAtivoRepository ativoRepository,
        IOperacaoRepository operacaoRepository,
        ICurrentUser currentUser)
    {
        _ativoRepository = ativoRepository;
        _operacaoRepository = operacaoRepository;
        _currentUser = currentUser;
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

        var usuarioId = _currentUser.UsuarioId
            ?? throw new InvalidOperationException("Usuário autenticado não encontrado.");

        var operacao = new Operacao(usuarioId, ativoId, tipo, data, quantidade, precoUnitario, taxas, observacao);
        var operacoes = (await _operacaoRepository.GetByAtivoIdAsync(ativoId)).Append(operacao);

        CalcularPosicaoAtivoService.Calcular(operacoes);

        await _operacaoRepository.AddAsync(operacao);
    }
}
