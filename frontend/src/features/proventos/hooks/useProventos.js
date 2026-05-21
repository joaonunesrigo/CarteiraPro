import { useProventosQuery } from '../queries/proventosQueries'

export function useProventos() {
  const query = useProventosQuery()

  return {
    proventos: query.data?.lista ?? [],
    resumo: query.data?.resumo ?? null,
    carregando: query.isLoading,
    erro: query.error?.message ?? null,
    recarregar: query.refetch,
  }
}
