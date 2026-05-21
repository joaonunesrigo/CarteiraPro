import { useQuery } from '@tanstack/react-query'
import { carteiraApi } from '../services/carteira.api'

export const carteiraQueryKey = ['carteira']

export function operacoesAtivoQueryKey(ativoId) {
  return ['ativos', ativoId, 'operacoes']
}

export function useCarteiraQuery() {
  return useQuery({
    queryKey: carteiraQueryKey,
    queryFn: carteiraApi.obterRentabilidade,
  })
}

export function useOperacoesAtivoQuery(ativoId) {
  return useQuery({
    queryKey: operacoesAtivoQueryKey(ativoId),
    queryFn: () => carteiraApi.listarOperacoes(ativoId),
    enabled: Boolean(ativoId),
  })
}
