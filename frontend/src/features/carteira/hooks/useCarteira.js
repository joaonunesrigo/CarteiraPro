import { useMemo } from 'react'
import { montarDadosGraficos } from '../utils/montarDadosGraficos'
import { formatarLinhasAtivos } from '../utils/formatarLinhasAtivos'
import { montarCartoesResumo } from '../utils/montarCartoesResumo'
import { montarResumoCarteira } from '../utils/montarResumoCarteira'
import { normalizarListaRentabilidade } from '../utils/normalizarRentabilidade'
import { useCarteiraQuery } from '../queries/carteiraQueries'
import { useCarteiraStore } from '../stores/carteiraStore'

function obterUltimaAtualizacaoCotacao(linhasAtivos) {
  const timestamps = linhasAtivos
    .map((ativo) => ativo.cotacaoAtualizadaEm)
    .filter(Boolean)
    .map((data) => new Date(data).getTime())
    .filter((timestamp) => !Number.isNaN(timestamp))

  if (timestamps.length === 0) return null

  return new Date(Math.max(...timestamps)).toISOString()
}

export function useCarteira() {
  const carteiraId = useCarteiraStore((state) => state.carteiraAtivaId)
  const query = useCarteiraQuery(carteiraId)
  const dados = useMemo(() => normalizarListaRentabilidade(query.data ?? []), [query.data])
  const resumo = useMemo(() => montarResumoCarteira(dados), [dados])
  const linhasAtivos = useMemo(() => formatarLinhasAtivos(dados), [dados])

  const cartoesResumo = montarCartoesResumo(resumo)
  const dadosGraficos = useMemo(() => montarDadosGraficos(linhasAtivos), [linhasAtivos])
  const cotacaoAtualizadaEm = useMemo(() => obterUltimaAtualizacaoCotacao(linhasAtivos), [linhasAtivos])

  return {
    resumo,
    cartoesResumo,
    linhasAtivos,
    dadosGraficos,
    carregando: query.isLoading,
    erro: query.error?.message ?? null,
    cotacaoAtualizadaEm,
    recarregar: query.refetch,
  }
}
