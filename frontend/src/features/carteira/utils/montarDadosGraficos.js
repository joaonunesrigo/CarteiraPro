import { COR_SEM_SETOR, CORES_GRAFICO, CORES_TIPO_ATIVO } from '../constants/graficos.constants'
import { TIPOS_ATIVO } from '../constants/tiposAtivo'

const ROTULO_TIPO_ATIVO = Object.fromEntries(TIPOS_ATIVO.map((t) => [t.valor, t.rotulo]))
const SETOR_NAO_INFORMADO = 'Não informado'

function arredondarPercentual(valor, total) {
  return total > 0 ? Math.round((valor / total) * 1000) / 10 : 0
}

function agruparPorSetor(linhas) {
  if (linhas.length === 0) return []

  const totalGrupo = linhas.reduce((soma, ativo) => soma + Number(ativo.valorAtual), 0)
  const grupos = new Map()

  for (const ativo of linhas) {
    const setor = ativo.setor?.trim() || SETOR_NAO_INFORMADO
    const grupo = grupos.get(setor) ?? { valor: 0, ativos: [] }
    grupo.valor += Number(ativo.valorAtual)
    grupo.ativos.push({ ticker: ativo.ticker, valor: Number(ativo.valorAtual) })
    grupos.set(setor, grupo)
  }

  return [...grupos.entries()]
    .sort((a, b) => b[1].valor - a[1].valor)
    .map(([setor, grupo], indice) => ({
      setor,
      valor: grupo.valor,
      percentual: arredondarPercentual(grupo.valor, totalGrupo),
      cor: setor === SETOR_NAO_INFORMADO ? COR_SEM_SETOR : CORES_GRAFICO[indice % CORES_GRAFICO.length],
      ativos: grupo.ativos.sort((a, b) => b.valor - a.valor),
    }))
}

export function montarDadosGraficos(linhas) {
  if (!linhas?.length) {
    return {
      temDados: false,
      alocacao: [],
      alocacaoPorTipo: [],
      alocacaoPorSetorAcoes: [],
      alocacaoPorSetorFiis: [],
      comparativo: [],
      rentabilidade: [],
    }
  }

  const totalAtual = linhas.reduce((soma, ativo) => soma + Number(ativo.valorAtual), 0)

  const alocacao = linhas.map((ativo, indice) => ({
    ticker: ativo.ticker,
    valor: Number(ativo.valorAtual),
    percentual: arredondarPercentual(Number(ativo.valorAtual), totalAtual),
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
      percentual: arredondarPercentual(valor, totalAtual),
      cor: CORES_TIPO_ATIVO[Number(tipo)] ?? COR_SEM_SETOR,
    }))
    .sort((a, b) => b.valor - a.valor)

  const acoes = linhas.filter((ativo) => Number(ativo.tipo ?? 0) === 0)
  const fiis = linhas.filter((ativo) => Number(ativo.tipo ?? 0) === 1)
  const alocacaoPorSetorAcoes = agruparPorSetor(acoes)
  const alocacaoPorSetorFiis = agruparPorSetor(fiis)

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
    alocacaoPorSetorAcoes,
    alocacaoPorSetorFiis,
    comparativo,
    rentabilidade,
  }
}
