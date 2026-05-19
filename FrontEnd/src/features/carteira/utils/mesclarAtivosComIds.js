export function mesclarAtivosComIds(rentabilidade, ativos) {
  const idPorTicker = Object.fromEntries(ativos.map((a) => [a.ticker, a.id]))
  return rentabilidade.map((item) => ({
    ...item,
    id: idPorTicker[item.ticker],
  }))
}
