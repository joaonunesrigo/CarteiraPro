import {
  normalizarTicker,
  validarTickerDuplicado,
} from './validarTicker'

export function validarFormularioAtivo(formulario, tickersCadastrados = []) {
  const erros = {}

  const ticker = normalizarTicker(formulario.ticker)
  if (!ticker) {
    erros.ticker = 'Informe um ticker.'
  } else {
    const erroDuplicado = validarTickerDuplicado(ticker, tickersCadastrados)
    if (erroDuplicado) erros.ticker = erroDuplicado
  }

  if (formulario.precoMedio === '' || formulario.precoMedio === null) {
    erros.precoMedio = 'Informe o preço médio.'
  } else if (Number(formulario.precoMedio) <= 0) {
    erros.precoMedio = 'O preço médio deve ser maior que zero.'
  }

  if (formulario.quantidade === '' || formulario.quantidade === null) {
    erros.quantidade = 'Informe a quantidade.'
  } else if (Number(formulario.quantidade) <= 0) {
    erros.quantidade = 'A quantidade deve ser maior que zero.'
  }

  return erros
}

export function formularioTemErros(erros) {
  return Object.keys(erros).length > 0
}
