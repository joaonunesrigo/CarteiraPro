using Application.Exceptions;
using Application.Services.Operacoes.DTOs;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;

namespace Application.Services.Operacoes;

public class CalcularPosicaoAtivoService
{
    private readonly IOperacaoRepository _operacaoRepository;

    public CalcularPosicaoAtivoService(IOperacaoRepository operacaoRepository)
    {
        _operacaoRepository = operacaoRepository;
    }

    public async Task<PosicaoAtivoDto> ExecuteAsync(Guid ativoId)
    {
        var operacoes = await _operacaoRepository.GetByAtivoIdAsync(ativoId);
        return Calcular(operacoes);
    }

    public async Task<IReadOnlyDictionary<Guid, PosicaoAtivoDto>> ExecuteAsync(IEnumerable<Ativo> ativos)
    {
        var listaAtivos = ativos.ToList();
        var idsAtivos = listaAtivos.Select(a => a.Id).ToHashSet();
        var carteiraIds = listaAtivos.Select(a => a.CarteiraId).Distinct().ToList();
        var todasOperacoes = new List<Operacao>();

        foreach (var carteiraId in carteiraIds)
            todasOperacoes.AddRange(await _operacaoRepository.GetAllAsync(carteiraId));

        var operacoes = todasOperacoes
            .Where(o => idsAtivos.Contains(o.AtivoId))
            .GroupBy(o => o.AtivoId)
            .ToDictionary(g => g.Key, g => Calcular(g));

        return idsAtivos.ToDictionary(
            id => id,
            id => operacoes.TryGetValue(id, out var posicao)
                ? posicao
                : new PosicaoAtivoDto(0, 0, 0, 0));
    }

    public static PosicaoAtivoDto Calcular(IEnumerable<Operacao> operacoes)
    {
        decimal quantidade = 0;
        decimal precoMedio = 0;
        decimal ganhoRealizado = 0;

        foreach (var operacao in Ordenar(operacoes))
        {
            if (operacao.Quantidade <= 0)
                throw new OperacaoInvalidaException("Quantidade da operação deve ser maior que zero.");

            if (operacao.PrecoUnitario <= 0)
                throw new OperacaoInvalidaException("Preço unitário da operação deve ser maior que zero.");

            if (operacao.Taxas < 0)
                throw new OperacaoInvalidaException("Taxas não podem ser negativas.");

            if (operacao.Tipo == TipoOperacao.Compra)
            {
                var custoAtual = quantidade * precoMedio;
                var custoCompra = operacao.ValorBruto + operacao.Taxas;
                quantidade += operacao.Quantidade;
                precoMedio = quantidade > 0 ? (custoAtual + custoCompra) / quantidade : 0;
                continue;
            }

            if (operacao.Quantidade > quantidade)
                throw new OperacaoInvalidaException("Venda não pode deixar a posição do ativo negativa.");

            var custoMedioVendido = precoMedio * operacao.Quantidade;
            ganhoRealizado += operacao.ValorLiquido - custoMedioVendido;
            quantidade -= operacao.Quantidade;

            if (quantidade == 0)
                precoMedio = 0;
        }

        return new PosicaoAtivoDto(
            quantidade,
            Math.Round(precoMedio, 6, MidpointRounding.AwayFromZero),
            Math.Round(quantidade * precoMedio, 2, MidpointRounding.AwayFromZero),
            Math.Round(ganhoRealizado, 2, MidpointRounding.AwayFromZero));
    }

    private static IEnumerable<Operacao> Ordenar(IEnumerable<Operacao> operacoes)
    {
        return operacoes
            .OrderBy(o => o.Data)
            .ThenBy(o => o.DataCadastro)
            .ThenBy(o => o.Id);
    }
}
