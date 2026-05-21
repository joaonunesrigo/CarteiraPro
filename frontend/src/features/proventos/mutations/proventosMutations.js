import { useMutation, useQueryClient } from '@tanstack/react-query'
import { proventosQueryKey } from '../queries/proventosQueries'
import { proventosApi } from '../services/proventos.api'

export function useImportarProventosMutation() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: proventosApi.importar,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: proventosQueryKey })
    },
  })
}

export function useRemoverProventoMutation() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: proventosApi.remover,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: proventosQueryKey })
    },
  })
}
