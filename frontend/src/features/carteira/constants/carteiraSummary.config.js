import { formatarMoeda, formatarPercentual } from '../../../utils/format'

export const CONFIG_CARTOES_RESUMO = [
  {
    rotulo: 'Total investido',
    chave: 'totalInvestido',
    formatar: formatarMoeda,
  },
  { rotulo: 'Valor atual', chave: 'totalAtual', formatar: formatarMoeda },
  {
    rotulo: 'Rentabilidade (R$)',
    chave: 'rentabilidadeReais',
    formatar: formatarMoeda,
    comSinal: true,
  },
  {
    rotulo: 'Rentabilidade (%)',
    chave: 'rentabilidadePercent',
    formatar: formatarPercentual,
    comSinal: true,
  },
]
