const MESES_ABREVIADOS = ['jan', 'fev', 'mar', 'abr', 'mai', 'jun', 'jul', 'ago', 'set', 'out', 'nov', 'dez']

function formatarRotulo(data) {
  return `${MESES_ABREVIADOS[data.getUTCMonth()]}/${String(data.getUTCFullYear()).slice(2)}`
}

export function normalizarEvolucao(payload) {
  const pontos = payload?.pontos ?? payload?.Pontos ?? []
  const tickersSemHistorico = payload?.tickersSemHistorico ?? payload?.TickersSemHistorico ?? []

  const pontosNormalizados = pontos.map((ponto) => {
    const dataIso = ponto.data ?? ponto.Data
    const data = new Date(dataIso)
    return {
      dataIso,
      rotulo: formatarRotulo(data),
      patrimonio: Number(ponto.patrimonio ?? ponto.Patrimonio ?? 0),
      valorInvestido: Number(ponto.valorInvestido ?? ponto.ValorInvestido ?? 0),
    }
  })

  return {
    pontos: pontosNormalizados,
    tickersSemHistorico,
  }
}
