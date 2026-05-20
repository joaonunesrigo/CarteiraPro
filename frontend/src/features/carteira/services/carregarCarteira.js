import { carteiraApi } from './carteira.api'

let promessaEmAndamento = null

export function carregarCarteira(opcoes = {}) {
  if (promessaEmAndamento) {
    return promessaEmAndamento
  }

  promessaEmAndamento = carteiraApi
    .obterRentabilidade(opcoes)
    .finally(() => {
      promessaEmAndamento = null
    })

  return promessaEmAndamento
}

export function cancelarCarregamentoCarteira() {
  promessaEmAndamento = null
}
