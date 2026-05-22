import { useQuery } from '@tanstack/react-query'
import { proventosApi } from '../services/proventos.api'

export function proventosQueryKey(carteiraId) {
  return ['proventos', carteiraId ?? 'padrao']
}

export function useProventosQuery(carteiraId) {
  return useQuery({
    queryKey: proventosQueryKey(carteiraId),
    queryFn: async () => {
      const [lista, resumo] = await Promise.all([proventosApi.listar(carteiraId), proventosApi.obterResumo(carteiraId)])

      return { lista, resumo }
    },
  })
}
