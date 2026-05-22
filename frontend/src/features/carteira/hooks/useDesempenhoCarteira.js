import { useMemo, useState } from 'react'
import { useEvolucaoCarteiraQuery } from '../queries/carteiraQueries'
import { PERIODOS_DESEMPENHO, PERIODO_DESEMPENHO_PADRAO } from '../constants/periodosDesempenho'
import { normalizarEvolucao } from '../utils/normalizarEvolucao'
import { useCarteiraStore } from '../stores/carteiraStore'

function obterPeriodo(id) {
  return PERIODOS_DESEMPENHO.find((p) => p.id === id) ?? PERIODOS_DESEMPENHO[2]
}

export function useDesempenhoCarteira() {
  const [periodoId, setPeriodoId] = useState(PERIODO_DESEMPENHO_PADRAO)
  const carteiraId = useCarteiraStore((state) => state.carteiraAtivaId)
  const periodo = obterPeriodo(periodoId)
  const query = useEvolucaoCarteiraQuery(periodo.meses, carteiraId)

  const evolucao = useMemo(() => normalizarEvolucao(query.data), [query.data])

  return {
    periodos: PERIODOS_DESEMPENHO,
    periodoSelecionado: periodoId,
    selecionarPeriodo: setPeriodoId,
    pontos: evolucao.pontos,
    tickersSemHistorico: evolucao.tickersSemHistorico,
    carregando: query.isLoading,
    erro: query.error?.message ?? null,
  }
}
