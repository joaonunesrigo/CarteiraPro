import { CORES_GRAFICO, CORES_TIPO_ATIVO } from '../constants/graficos.constants'
import { TIPOS_ATIVO } from '../constants/tiposAtivo'

const ROTULO_TIPO_ATIVO = Object.fromEntries(TIPOS_ATIVO.map((t) => [t.valor, t.rotulo]))

export function montarDadosGraficos(linhas) {
  if (!linhas?.length) {
    return {
      temDados: false,
      alocacao: [],
      alocacaoPorTipo: [],
      comparativo: [],
      rentabilidade: [],
    }
  }

  const totalAtual = linhas.reduce((soma, ativo) => soma + Number(ativo.valorAtual), 0)

  const alocacao = linhas.map((ativo, indice) => ({
    ticker: ativo.ticker,
    valor: Number(ativo.valorAtual),
    percentual: totalAtual > 0 ? Math.round((Number(ativo.valorAtual) / totalAtual) * 1000) / 10 : 0,
    cor: CORES_GRAFICO[indice % CORES_GRAFICO.length],
  }))

  const totaisPorTipo = linhas.reduce((mapa, ativo) => {
    const tipo = Number(ativo.tipo ?? 0)
    mapa[tipo] = (mapa[tipo] ?? 0) + Number(ativo.valorAtual)
    return mapa
  }, {})

  const alocacaoPorTipo = Object.entries(totaisPorTipo)
    .map(([tipo, valor]) => ({
      tipo: Number(tipo),
      rotulo: ROTULO_TIPO_ATIVO[Number(tipo)] ?? 'Outros',
      valor,
      percentual: totalAtual > 0 ? Math.round((valor / totalAtual) * 1000) / 10 : 0,
      cor: CORES_TIPO_ATIVO[Number(tipo)] ?? '#94a3b8',
    }))
    .sort((a, b) => b.valor - a.valor)

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
    alocacaoPorTipo,
    comparativo,
    rentabilidade,
  }
}
