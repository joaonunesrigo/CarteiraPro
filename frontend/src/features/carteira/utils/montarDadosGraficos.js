import { CORES_GRAFICO } from '../constants/graficos.constants'

export function montarDadosGraficos(linhas) {
  if (!linhas?.length) {
    return {
      temDados: false,
      alocacao: [],
      comparativo: [],
      rentabilidade: [],
    }
  }

  const totalAtual = linhas.reduce(
    (soma, ativo) => soma + Number(ativo.valorAtual),
    0,
  )

  const alocacao = linhas.map((ativo, indice) => ({
    ticker: ativo.ticker,
    valor: Number(ativo.valorAtual),
    percentual:
      totalAtual > 0
        ? Math.round((Number(ativo.valorAtual) / totalAtual) * 1000) / 10
        : 0,
    cor: CORES_GRAFICO[indice % CORES_GRAFICO.length],
  }))

  const comparativo = linhas.map((ativo) => ({
    ticker: ativo.ticker,
    investido: Number(ativo.valorInvestido),
    atual: Number(ativo.valorAtual),
  }))

  const rentabilidade = linhas.map((ativo) => ({
    ticker: ativo.ticker,
    percentual: Number(ativo.rentabilidadePercent),
    reais: Number(ativo.rentabilidadeReais),
    positivo: Number(ativo.rentabilidadePercent) >= 0,
  }))

  return {
    temDados: true,
    alocacao,
    comparativo,
    rentabilidade,
  }
}
