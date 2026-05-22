export const MOEDA_BRL = 0
const MOEDA_USD = 1

export const MOEDAS = [
  { valor: MOEDA_BRL, rotulo: 'Real (BRL)', sigla: 'BRL' },
  { valor: MOEDA_USD, rotulo: 'Dólar (USD)', sigla: 'USD' },
]

export function obterSiglaMoeda(valor) {
  return MOEDAS.find((moeda) => moeda.valor === valor)?.sigla ?? 'BRL'
}
