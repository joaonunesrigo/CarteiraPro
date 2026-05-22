import { useProventosQuery } from '../queries/proventosQueries'
import { useCarteiraStore } from '../../carteira/stores/carteiraStore'

export function useProventos() {
  const carteiraId = useCarteiraStore((state) => state.carteiraAtivaId)
  const query = useProventosQuery(carteiraId)

  return {
    proventos: query.data?.lista ?? [],
    resumo: query.data?.resumo ?? null,
    carregando: query.isLoading,
    erro: query.error?.message ?? null,
    recarregar: query.refetch,
  }
}
