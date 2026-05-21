import { useQuery } from '@tanstack/react-query'
import { proventosApi } from '../services/proventos.api'

export const proventosQueryKey = ['proventos']

export function useProventosQuery() {
  return useQuery({
    queryKey: proventosQueryKey,
    queryFn: async () => {
      const [lista, resumo] = await Promise.all([proventosApi.listar(), proventosApi.obterResumo()])

      return { lista, resumo }
    },
  })
}
