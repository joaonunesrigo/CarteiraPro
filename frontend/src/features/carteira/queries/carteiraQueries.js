import { useQuery } from '@tanstack/react-query'
import { carteiraApi } from '../services/carteira.api'
import { mercadoAberto } from '../utils/mercadoAberto'

export const carteiraQueryKey = ['carteira']
const INTERVALO_COTACOES_PREGAO = 30 * 60 * 1000

export function operacoesAtivoQueryKey(ativoId) {
  return ['ativos', ativoId, 'operacoes']
}

export function evolucaoCarteiraQueryKey(meses) {
  return ['carteira', 'evolucao', meses]
}

export function useCarteiraQuery() {
  return useQuery({
    queryKey: carteiraQueryKey,
    queryFn: carteiraApi.obterRentabilidade,
    refetchInterval: () => (mercadoAberto() ? INTERVALO_COTACOES_PREGAO : false),
    refetchIntervalInBackground: false,
  })
}

export function useOperacoesAtivoQuery(ativoId) {
  return useQuery({
    queryKey: operacoesAtivoQueryKey(ativoId),
    queryFn: () => carteiraApi.listarOperacoes(ativoId),
    enabled: Boolean(ativoId),
  })
}

export function useEvolucaoCarteiraQuery(meses) {
  return useQuery({
    queryKey: evolucaoCarteiraQueryKey(meses),
    queryFn: () => carteiraApi.obterEvolucao(meses),
    staleTime: 10 * 60 * 1000,
  })
}
