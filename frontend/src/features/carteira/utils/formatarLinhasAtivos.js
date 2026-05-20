import { formatarMoeda, formatarPercentual } from '../../../utils/format'

export function formatarLinhasAtivos(linhas) {
  return linhas.map((ativo) => ({
    ...ativo,
    precoMedioFormatado: formatarMoeda(ativo.precoMedio),
    cotacaoFormatada: formatarMoeda(ativo.cotacaoAtual),
    rentabilidadePercentFormatada: formatarPercentual(ativo.rentabilidadePercent),
    rentabilidadeReaisFormatada: formatarMoeda(ativo.rentabilidadeReais),
    rentabilidadePercentPositiva: ativo.rentabilidadePercent >= 0,
    rentabilidadeReaisPositiva: ativo.rentabilidadeReais >= 0,
  }))
}
