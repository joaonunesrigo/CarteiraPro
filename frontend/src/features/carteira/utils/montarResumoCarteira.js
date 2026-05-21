export function montarResumoCarteira(rentabilidade) {
  const linhas = Array.isArray(rentabilidade) ? rentabilidade : []

  const totalInvestido = linhas.reduce((soma, ativo) => soma + Number(ativo.valorInvestido), 0)
  const totalAtual = linhas.reduce((soma, ativo) => soma + Number(ativo.valorAtual), 0)
  const rentabilidadeReais = linhas.reduce((soma, ativo) => soma + Number(ativo.rentabilidadeReais), 0)
  const rentabilidadePercent = totalInvestido > 0 ? Math.round((rentabilidadeReais / totalInvestido) * 10000) / 100 : 0

  return {
    totalInvestido,
    totalAtual,
    rentabilidadeReais,
    rentabilidadePercent,
  }
}
