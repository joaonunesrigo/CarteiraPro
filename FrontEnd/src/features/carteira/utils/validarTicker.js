export function normalizarTicker(ticker) {
  return ticker.trim().toUpperCase()
}

export function validarTickerObrigatorio(ticker) {
  if (!normalizarTicker(ticker)) {
    return 'Informe um ticker.'
  }
  return null
}

export function validarTickerDuplicado(ticker, tickersCadastrados) {
  const tickerNormalizado = normalizarTicker(ticker)
  if (tickersCadastrados.includes(tickerNormalizado)) {
    return `O ticker ${tickerNormalizado} já está na carteira.`
  }
  return null
}
