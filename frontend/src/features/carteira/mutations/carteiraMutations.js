import { useMutation, useQueryClient } from '@tanstack/react-query'
import { carteiraQueryKey, carteirasQueryKey, operacoesAtivoQueryKey } from '../queries/carteiraQueries'
import { carteiraApi } from '../services/carteira.api'
import { useCarteiraStore } from '../stores/carteiraStore'

export function useAdicionarCarteiraMutation() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: carteiraApi.adicionarCarteira,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: carteirasQueryKey() })
    },
  })
}

export function useAtualizarCarteiraMutation() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: ({ id, dados }) => carteiraApi.atualizarCarteira(id, dados),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: carteirasQueryKey() })
    },
  })
}

export function useRemoverCarteiraMutation() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: carteiraApi.removerCarteira,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: carteirasQueryKey() })
      queryClient.invalidateQueries({ queryKey: ['carteira'] })
      queryClient.invalidateQueries({ queryKey: ['proventos'] })
    },
  })
}

export function useAdicionarAtivoMutation() {
  const queryClient = useQueryClient()
  const carteiraId = useCarteiraStore((state) => state.carteiraAtivaId)

  return useMutation({
    mutationFn: (dados) => carteiraApi.adicionarAtivo(dados, carteiraId),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: carteiraQueryKey(carteiraId) })
    },
  })
}

export function useRemoverAtivoMutation() {
  const queryClient = useQueryClient()
  const carteiraId = useCarteiraStore((state) => state.carteiraAtivaId)

  return useMutation({
    mutationFn: carteiraApi.removerAtivo,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: carteiraQueryKey(carteiraId) })
    },
  })
}

export function useRemoverTodosAtivosMutation() {
  const queryClient = useQueryClient()
  const carteiraId = useCarteiraStore((state) => state.carteiraAtivaId)

  return useMutation({
    mutationFn: () => carteiraApi.removerTodosAtivos(carteiraId),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: carteiraQueryKey(carteiraId) })
    },
  })
}

export function useImportarAtivosMutation() {
  const queryClient = useQueryClient()
  const carteiraId = useCarteiraStore((state) => state.carteiraAtivaId)

  return useMutation({
    mutationFn: (dados) => carteiraApi.importarAtivos(dados, carteiraId),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: carteiraQueryKey(carteiraId) })
    },
  })
}

export function useAdicionarOperacaoMutation(ativoId) {
  const queryClient = useQueryClient()
  const carteiraId = useCarteiraStore((state) => state.carteiraAtivaId)

  return useMutation({
    mutationFn: (dados) => carteiraApi.adicionarOperacao(ativoId, dados),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: carteiraQueryKey(carteiraId) })
      queryClient.invalidateQueries({
        queryKey: operacoesAtivoQueryKey(ativoId),
      })
    },
  })
}

export function useRemoverOperacaoMutation(ativoId) {
  const queryClient = useQueryClient()
  const carteiraId = useCarteiraStore((state) => state.carteiraAtivaId)

  return useMutation({
    mutationFn: carteiraApi.removerOperacao,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: carteiraQueryKey(carteiraId) })
      queryClient.invalidateQueries({
        queryKey: operacoesAtivoQueryKey(ativoId),
      })
    },
  })
}
