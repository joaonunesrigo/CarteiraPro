import { useMemo } from 'react'
import { montarDadosGraficos } from '../utils/montarDadosGraficos'
import { formatarLinhasAtivos } from '../utils/formatarLinhasAtivos'
import { montarCartoesResumo } from '../utils/montarCartoesResumo'
import { montarResumoCarteira } from '../utils/montarResumoCarteira'
import { normalizarListaRentabilidade } from '../utils/normalizarRentabilidade'
import { useCarteiraQuery } from '../queries/carteiraQueries'

export function useCarteira() {
  const query = useCarteiraQuery()
  const dados = useMemo(() => normalizarListaRentabilidade(query.data ?? []), [query.data])
  const resumo = useMemo(() => montarResumoCarteira(dados), [dados])
  const linhasAtivos = useMemo(() => formatarLinhasAtivos(dados), [dados])

  const cartoesResumo = montarCartoesResumo(resumo)
  const dadosGraficos = useMemo(() => montarDadosGraficos(linhasAtivos), [linhasAtivos])

  return {
    resumo,
    cartoesResumo,
    linhasAtivos,
    dadosGraficos,
    carregando: query.isLoading,
    erro: query.error?.message ?? null,
    recarregar: query.refetch,
  }
}
