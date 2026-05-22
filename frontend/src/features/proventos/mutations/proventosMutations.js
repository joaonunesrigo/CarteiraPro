import { useMutation, useQueryClient } from '@tanstack/react-query'
import { proventosQueryKey } from '../queries/proventosQueries'
import { proventosApi } from '../services/proventos.api'
import { useCarteiraStore } from '../../carteira/stores/carteiraStore'

export function useImportarProventosMutation() {
  const queryClient = useQueryClient()
  const carteiraId = useCarteiraStore((state) => state.carteiraAtivaId)

  return useMutation({
    mutationFn: (dados) => proventosApi.importar(dados, carteiraId),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: proventosQueryKey(carteiraId) })
    },
  })
}

export function useRemoverProventoMutation() {
  const queryClient = useQueryClient()
  const carteiraId = useCarteiraStore((state) => state.carteiraAtivaId)

  return useMutation({
    mutationFn: proventosApi.remover,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: proventosQueryKey(carteiraId) })
    },
  })
}
