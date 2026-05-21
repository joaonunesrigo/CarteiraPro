export function normalizarRentabilidade(item) {
  return {
    id: item.id ?? item.Id,
    ticker: item.ticker ?? item.Ticker,
    tipo: Number(item.tipo ?? item.Tipo ?? 0),
    precoMedio: Number(item.precoMedio ?? item.PrecoMedio ?? 0),
    cotacaoAtual: Number(item.cotacaoAtual ?? item.CotacaoAtual ?? 0),
    quantidade: Number(item.quantidade ?? item.Quantidade ?? 0),
    valorInvestido: Number(item.valorInvestido ?? item.ValorInvestido ?? 0),
    valorAtual: Number(item.valorAtual ?? item.ValorAtual ?? 0),
    rentabilidadeReais: Number(item.rentabilidadeReais ?? item.RentabilidadeReais ?? 0),
    rentabilidadePercent: Number(item.rentabilidadePercent ?? item.RentabilidadePercent ?? 0),
  }
}

export function normalizarListaRentabilidade(lista) {
  if (!Array.isArray(lista)) return []
  return lista.map(normalizarRentabilidade)
}
