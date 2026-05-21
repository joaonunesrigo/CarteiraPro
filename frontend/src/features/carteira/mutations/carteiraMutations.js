import { useMutation, useQueryClient } from '@tanstack/react-query'
import { carteiraQueryKey, operacoesAtivoQueryKey } from '../queries/carteiraQueries'
import { carteiraApi } from '../services/carteira.api'

export function useAdicionarAtivoMutation() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: carteiraApi.adicionarAtivo,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: carteiraQueryKey })
    },
  })
}

export function useRemoverAtivoMutation() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: carteiraApi.removerAtivo,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: carteiraQueryKey })
    },
  })
}

export function useRemoverTodosAtivosMutation() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: carteiraApi.removerTodosAtivos,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: carteiraQueryKey })
    },
  })
}

export function useImportarAtivosMutation() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: carteiraApi.importarAtivos,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: carteiraQueryKey })
    },
  })
}

export function useAdicionarOperacaoMutation(ativoId) {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (dados) => carteiraApi.adicionarOperacao(ativoId, dados),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: carteiraQueryKey })
      queryClient.invalidateQueries({
        queryKey: operacoesAtivoQueryKey(ativoId),
      })
    },
  })
}

export function useRemoverOperacaoMutation(ativoId) {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: carteiraApi.removerOperacao,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: carteiraQueryKey })
      queryClient.invalidateQueries({
        queryKey: operacoesAtivoQueryKey(ativoId),
      })
    },
  })
}
