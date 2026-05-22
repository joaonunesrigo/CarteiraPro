import { useQuery } from '@tanstack/react-query'
import { carteiraApi } from '../services/carteira.api'
import { mercadoAberto } from '../utils/mercadoAberto'

export function carteirasQueryKey() {
  return ['carteiras']
}

export function carteiraQueryKey(carteiraId) {
  return ['carteira', carteiraId ?? 'padrao']
}
const INTERVALO_COTACOES_PREGAO = 30 * 60 * 1000

export function operacoesAtivoQueryKey(ativoId) {
  return ['ativos', ativoId, 'operacoes']
}

export function evolucaoCarteiraQueryKey(meses, carteiraId) {
  return ['carteira', carteiraId ?? 'padrao', 'evolucao', meses]
}

export function useCarteirasQuery() {
  return useQuery({
    queryKey: carteirasQueryKey(),
    queryFn: carteiraApi.listarCarteiras,
  })
}

export function useCarteiraQuery(carteiraId) {
  return useQuery({
    queryKey: carteiraQueryKey(carteiraId),
    queryFn: () => carteiraApi.obterRentabilidade(carteiraId),
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

export function useEvolucaoCarteiraQuery(meses, carteiraId) {
  return useQuery({
    queryKey: evolucaoCarteiraQueryKey(meses, carteiraId),
    queryFn: () => carteiraApi.obterEvolucao(meses, carteiraId),
    staleTime: 10 * 60 * 1000,
  })
}
